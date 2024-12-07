// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "I don't want to use the folder structure.", Scope = "namespace", Target = "~N:Microsoft.AspNetCore.Builder")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "I don't want to use the folder structure.", Scope = "namespace", Target = "~N:Microsoft.Extensions.DependencyInjection")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "I don't want to use the folder structure.", Scope = "namespace", Target = "~N:Rystem.PlayFramework")]
[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "I don't want to use Primary Constructor.", Scope = "member", Target = "~M:Rystem.PlayFramework.SceneManager.#ctor(System.IServiceProvider,Microsoft.AspNetCore.Http.IHttpContextAccessor,Microsoft.Extensions.DependencyInjection.IFactory{Rystem.OpenAi.IOpenAi},Microsoft.Extensions.DependencyInjection.IFactory{Rystem.PlayFramework.IScene},System.Net.Http.IHttpClientFactory,Rystem.PlayFramework.PlayHandler,Rystem.PlayFramework.FunctionsHandler,Rystem.PlayFramework.SceneManagerSettings)")]
[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "I don't want to use Primary Constructor.", Scope = "member", Target = "~M:Rystem.PlayFramework.StreamingSceneManager.#ctor(System.IServiceProvider,Microsoft.AspNetCore.Http.IHttpContextAccessor,Microsoft.Extensions.DependencyInjection.IFactory{Rystem.OpenAi.IOpenAi},Microsoft.Extensions.DependencyInjection.IFactory{Rystem.PlayFramework.IScene},System.Net.Http.IHttpClientFactory,Rystem.PlayFramework.PlayHandler,Rystem.PlayFramework.FunctionsHandler,Rystem.PlayFramework.SceneManagerSettings)")]
