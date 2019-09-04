using System;



namespace POCSample
{

    using CommandLine;

    using System;
    /// <summary>

    /// Main samples entry point.

    /// </summary>
    /// 

    class BackEndOptions

    {

        [Option('u', "uri", HelpText = "Please specify the uri of backend", Required = true)]

        public string Uri { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {


            Parser.Default.ParseArguments<BackEndOptions>(args)

             .MapResult(

                 (BackEndOptions options) => TestMSCore.Run(options.Uri),

                 errs => 1);



            Console.ReadLine();
        }
    }
}
