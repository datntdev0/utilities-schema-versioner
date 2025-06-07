namespace datntdev.SchemaVersioner.Models
{
    internal class CommandOutput
    {
        public CommandOutputInfo? Info { get; set; }
        public CommandOutputInit? Init { get; set; }
        public CommandOutputUpgrade? Upgrade { get; set; }
        public CommandOutputDowngrade? Downgrade { get; set; }
        public CommandOutputValidate? Validate { get; set; }
        public CommandOutputRepair? Repair { get; set; }
        public CommandOutputSnapshot? Snapshot { get; set; }

        public CommandOutput(CommandOutputInfo info) => Info = info;
        public CommandOutput(CommandOutputInit init) => Init = init;
        public CommandOutput(CommandOutputUpgrade upgrade) => Upgrade = upgrade;
        public CommandOutput(CommandOutputDowngrade downgrade) => Downgrade = downgrade;
        public CommandOutput(CommandOutputValidate validate) => Validate = validate;
        public CommandOutput(CommandOutputRepair repair) => Repair = repair;
        public CommandOutput(CommandOutputSnapshot snapshot) => Snapshot = snapshot;
    }

    internal class CommandOutputInfo
    {

    }

    internal class CommandOutputInit
    {

    }

    internal class CommandOutputUpgrade
    {

    }

    internal class CommandOutputDowngrade
    {

    }

    internal class CommandOutputValidate
    {

    }

    internal class CommandOutputRepair
    {

    }

    internal class CommandOutputSnapshot
    {

    }
}
