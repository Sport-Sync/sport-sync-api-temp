using System.Numerics;
using SportSync.Application.Authentication;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Cryptography;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Users.CreateUser;

public class CreateUserInputHandler : IInputHandler<CreateUserInput, TokenResponse>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserInputHandler(
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenResponse> Handle(CreateUserInput input, CancellationToken cancellationToken)
    {
        if (!await _userRepository.IsEmailUniqueAsync(input.Email))
        {
            throw new DomainException(DomainErrors.User.DuplicateEmail);
        }

        input.Phone = input.Phone.Replace("+385", "0");

        if (!await _userRepository.IsPhoneUniqueAsync(input.Phone))
        {
            throw new DomainException(DomainErrors.User.DuplicatePhone);
        }

        var passwordResult = Password.Create(input.Password);

        if (passwordResult.IsFailure)
        {
            throw new DomainException(passwordResult.Error);
        }

        var passwordHash = _passwordHasher.HashPassword(passwordResult.Value);

        var user = User.Create(input.FirstName, input.LastName, input.Email, input.Phone, passwordHash);

        _userRepository.Insert(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtProvider.Create(user);

        return new TokenResponse(token);
    }
}