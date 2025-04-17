using System;
using System.Collections.Generic;
using R3name.Models;
using R3name.Modules.FileSources;

namespace R3name.Modules.Validators;

class NotSupportedNameValidator : Validator
{
    public override IEnumerable<ValidationError> ValidateList(IEnumerable<IFileDescription> files)
    {
        throw new NotImplementedException();
    }
}