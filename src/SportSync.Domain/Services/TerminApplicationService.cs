using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Domain.Services;

public class TerminApplicationService
{
    private readonly IUserRepository _userRepository;

    public TerminApplicationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task CreateTerminApplication(User user, Termin termin)
    {

    }
}