using System.Diagnostics;
using Microsoft.Owin;
using Owin;
using LightNode.Server;
using LightNode.Formatter;
using Glimpse.LightNode;
using LightNode.Swagger;
using Noroshi.Server.Formatter;

[assembly: OwinStartup(typeof(Noroshi.Server.Startup1))]
namespace Noroshi.Server
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            app.EnableGlimpse(); // This is Glimpse.LightNode's helper for enable Glimpse

            app.Map("/api", builder =>
            {
                //TODO Mespack修正後対応
                var options = new LightNodeOptions(AcceptVerbs.Get | AcceptVerbs.Post, new DefaultJsonFormatter())//Noroshi.Server.Formatter.DefaultFormatter())
                {
                    // for Glimpse Profiling
                    OperationCoordinatorFactory = new GlimpseProfilingOperationCoordinatorFactory(),

                    ParameterEnumAllowsFieldNameParse = true, // If you want to use enums human readable display on Swagger, set to true
                    ErrorHandlingPolicy = ErrorHandlingPolicy.ReturnInternalServerErrorIncludeErrorDetails,
                    OperationMissingHandlingPolicy = OperationMissingHandlingPolicy.ReturnErrorStatusCodeIncludeErrorDetails
                };
                options.Filters.Add(new Filters.ContextContainerHandler());
                builder.UseLightNode(options);
            });
#if DEBUG
            app.Map("/debug/api", builder =>
            {
                var options = new LightNodeOptions(AcceptVerbs.Get | AcceptVerbs.Post, new DataContractJsonContentFormatter())
                {
                    // for Glimpse Profiling
                    OperationCoordinatorFactory = new GlimpseProfilingOperationCoordinatorFactory(),

                    ParameterEnumAllowsFieldNameParse = true, // If you want to use enums human readable display on Swagger, set to true
                    ErrorHandlingPolicy = ErrorHandlingPolicy.ReturnInternalServerErrorIncludeErrorDetails,
                    OperationMissingHandlingPolicy = OperationMissingHandlingPolicy.ReturnErrorStatusCodeIncludeErrorDetails
                };
                options.Filters.Add(new Filters.ContextContainerHandler());
                builder.UseLightNode(options);
            });

            app.Map("/cheat/api", builder =>
            {
                //TODO Mespack修正後対応
                var options = new LightNodeOptions(AcceptVerbs.Get | AcceptVerbs.Post, new DefaultJsonFormatter())//Noroshi.Server.Formatter.DefaultFormatter())
                {
                    // for Glimpse Profiling
                    OperationCoordinatorFactory = new GlimpseProfilingOperationCoordinatorFactory(),

                    ParameterEnumAllowsFieldNameParse = true, // If you want to use enums human readable display on Swagger, set to true
                    ErrorHandlingPolicy = ErrorHandlingPolicy.ReturnInternalServerErrorIncludeErrorDetails,
                    OperationMissingHandlingPolicy = OperationMissingHandlingPolicy.ReturnErrorStatusCodeIncludeErrorDetails
                };
                options.Filters.Add(new Filters.ContextContainerHandler());
                builder.UseLightNode(options);
            });

            // Mapping to swagger path
            app.Map("/swagger", builder =>
            {
                // If you want to additional info for Swagger, load xmlDoc file.
                // LightNode.Swagger loads methods's summary, remarks, param for info.     
                //var xmlName = "LightNode.Sample.GlimpseUse.xml";
                //var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + xmlName; // or HttpContext.Current.Server.MapPath("~/bin/" + xmlName);

                builder.UseLightNodeSwagger(new SwaggerOptions("NoroshiServer", "/debug/api") // baseApi is LightNode's root
                {
                    //XmlDocumentPath = xmlPath,
                    IsEmitEnumAsString = true
                });
            });
#endif
        }
    }
}
