﻿// ------------------------------------------------------------------------
// Copyright 2021 The Dapr Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//     http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------

namespace Dapr.E2E.Test
{
    using Dapr.E2E.Test.Actors.Reentrancy;
    using Dapr.E2E.Test.Actors.Reminders;
    using Dapr.E2E.Test.Actors.Timers;
    using Dapr.E2E.Test.Actors.ExceptionTesting;
    using Dapr.E2E.Test.App.ErrorTesting;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures Services.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication().AddDapr();
            services.AddAuthorization(o => o.AddDapr());
            services.AddControllers().AddDapr();
            services.AddActors(options =>
            {
                options.Actors.RegisterActor<ReminderActor>();
                options.Actors.RegisterActor<TimerActor>();
                options.Actors.RegisterActor<Regression762Actor>();
                options.Actors.RegisterActor<ExceptionActor>();
            });
        }

        /// <summary>
        /// Configures Application Builder and WebHost environment.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Webhost environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapActorsHandlers();
                endpoints.MapControllers();
            });
        }
    }
}
