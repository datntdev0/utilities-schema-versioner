namespace datntdev.SchemaVersioner.Models
{
    public class CommandOutput { }

    public class CommandOutput<TOutput>(TOutput data) : CommandOutput where TOutput : class
    {
        public TOutput Data { get; set; } = data;
    }

    public class CommandOutputInfo
    {

    }

    public class CommandOutputInit
    {

    }

    public class CommandOutputUpgrade
    {

    }

    public class CommandOutputDowngrade
    {

    }

    public class CommandOutputValidate
    {

    }

    public class CommandOutputRepair
    {

    }

    public class CommandOutputErase
    {

    }

    public class CommandOutputSnapshot
    {

    }
}
