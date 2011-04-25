using System;
using System.ComponentModel;
using FubuCore.CommandLine;

namespace Bottles.Creation
{
    public enum CompileTargetEnum
    {
        debug,
        release
    }

    public class CreatePackageInput
    {
        public CreatePackageInput()
        {
            TargetFlag = CompileTargetEnum.debug;
        }

        [Description("The root physical folder (or valid alias) of the package")]
        public string PackageFolder { get; set; }

        [Description("The location where the zip file for the package will be written")]
        public string ZipFile { get; set; }

        [Description("Includes any matching .pdb files for the package assemblies")]
        public bool PdbFlag { get; set; }

        [Description("Forces the command to delete any existing zip file first")]
        [FlagAlias("f")]
        public bool ForceFlag { get; set; }

        [Description("Choose the compilation target for any assemblies")]
        public CompileTargetEnum TargetFlag { get; set; }
    }
}