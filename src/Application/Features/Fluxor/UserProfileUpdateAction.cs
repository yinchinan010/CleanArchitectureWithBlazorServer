﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Features.Fluxor;
public class UserProfileUpdateAction
{
    public required UserProfile UserProfile { get; set; }
}
