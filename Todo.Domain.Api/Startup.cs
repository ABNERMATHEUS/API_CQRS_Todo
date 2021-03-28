using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Todo.Domain.Handlers;
using Todo.Domain.Infra.DataContext;
using Todo.Domain.Infra.Repositories;
using Todo.Domain.Repositories;

namespace Todo.Domain.Api
{
    public class Startup
    {   
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.AddTransient  //Cria uma nova inst�ncia, toda vez que for pedir
            //services.AddScoped     //Um "Singleton" por requisi��o, quando finaliza ele se destroe, 1 conex�o por transa��es
            //services.AddSingleton  //Uma inst�ncia para a aplica��o, vai ficar ativa na mem�ria.(Sempre manter objeto na mem�ria)
            //services.AddDbContext  //� o mesmo addScoped, mas � espec�fico para trabalhar com banco de dados - Entity Framework Core

            //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
            services.AddDbContext<Context>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));
            services.AddTransient<ITodoRepository, TodoRepository>();
            services.AddTransient<TodoHandler, TodoHandler>();

            string urlValidationGoogle = "https://securetoken.google.com/todo-d5df5";
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.Authority = urlValidationGoogle; //URL DE AUTENTICA��O DO GOOGLE 
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, //Quero validar quem est� mandando o token ?
                        ValidIssuer = urlValidationGoogle, //Qual url que eu valido esse token ?
                        ValidateAudience = true, //Quero validar o app ?
                        ValidAudience = "todo-d5df5", //Qual projeto do app que valido ?
                        ValidateLifetime = true  //Valido o tempo de vida ? Se ele est� ativo ou n�o de fato 
                    };
                });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            //NECESS�RIO ESTAR QUANDO HOUVER AUTENTICA��O E AUTORIZA��O VIA TOKEN POR EXEMPLO
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
