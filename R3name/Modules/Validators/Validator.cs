using System.Collections.Generic;
using R3name.Models;
using R3name.Modules.FileSources;

namespace R3name.Modules.Validators;

public abstract class Validator
{
    public abstract IEnumerable<ValidationError> ValidateList(IEnumerable<IFileDescription> files);
}