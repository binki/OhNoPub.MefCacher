[![Build status](https://ci.appveyor.com/api/projects/status/fcqvbm756p7ojhy0?svg=true)](https://ci.appveyor.com/project/binki/ohnopub-mefcacher)

There are two goals to this project:

1. Enable the most general and declarative MEF usage pattern (i.e., using [`ApplicationCatalog`](https://msdn.microsoft.com/en-us/library/system.componentmodel.composition.hosting.applicationcatalog%28v=vs.110%29.aspx)).
   When buying into a composition pattern, it is really nice to let the system follow a well-established and accepted set of conventions to orchestrate parts.
   Writing your own initialization code to reduce startup delays means ignoring these conventions/patterns and ends up mixing concerns and defeating the whole point of a composition pattern.

2. Enable fast startup and initial composition.
   For MEF to compose a part with any imports, it needs to scan the entire catalog for parts to verify that the cardinality of the import is being respected.
   This defeats the framework痴 optimization of lazily loading assemblies and causes a noticable and unacceptable slowdown in program initialization, especially when allowing all assemblies in an application to contribute parts.

MEF痴 catalogs allow a separation between a part instance and its definition.
It should be possible to just feed MEF the part definitions necessary for it to validate cardinality constraints and decide which parts actually need to be activated.
This way, assembly loading can be deferred using a lazy pattern until parts are actually needed in a manner similar to how .net lazily loads assemblies.
If this is done, initial composition can be made responsive while still allowing a general catalog without need for manual tuning/specialization.

The design of `CachingCatalog` supports automatic cache rebuilding and initialization.
The idea is that you can detect that a particular assembly has changed without fully loading it用erhaps using filesystem-provided metadata such as mtime.
During normal application use, the assembly would be stable.
Thus, the program would build the cache on the first run and use it in subsequent runs.
Or perhaps the cache can be prepopulated before distribution of the program.

Because of the nature of the caching pattern, it is best if the cache can be assembly-aware.
Thus, imitations of `DirectoryCatalog` and `ApplicationCatalog` called `DirectoryCachingCatalog` and `ApplicationCachingCatalog` are provided.
The pattern I personally recommend is:

    using OhNoPub.MefCacher;
    using System.ComponentModel;
    using System.ComponentModel.Composition.Hosting;
    
    [Export]
    public class Program
    {
        [Import]
        public Whatever Whatever { get; set; }
        
        static int Main(string[] args)
        {
            using (var catalog = new ApplicationCachingCatalog())
            {
                using (var container = new CompositionContainer(catalog))
                {
                    return catalog.GetExportedValue<Program>().Run(args);
                }
            }
        }

        int Run(string[] args)
        {
            Whatever.DoSomething();
            return 0;
        }
    }

And… that is the vision.
It does not work quite so far yet.
This repository however has a few test cases and some of the necessary scaffolding to support the future API.
It might even almost be to the point of being interesting.
Thus I’m sharing it ^^.
