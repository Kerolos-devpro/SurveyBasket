global using Microsoft.AspNetCore.Mvc;
global using System.Reflection;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using System.Security.Claims;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Extensions.Options;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using MimeKit;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using System.Threading.RateLimiting;

global using Mapster;
global using FluentValidation;
global using MapsterMapper;
global using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;



global using SurveyBasket.Api.Entities;
global using SurveyBasket.Api.Services;
global using SurveyBasket.Api.Persistence;
global using SurveyBasket.Api.Contracts.Authentication;
global using SurveyBasket.Api.Authentication;
global using SurveyBasket.Api.Abstractions;
global using System.Security.Cryptography;
global using SurveyBasket.Api.Errors;
global using SurveyBasket.Api.Extensions;
global using SurveyBasket.Api.Contracts.Questions;
global using SurveyBasket.Api.Contracts.Results;
global using SurveyBasket.Api.Abstractions.Consts;
global using SurveyBasket.Api.Settings;
global using SurveyBasket.Api.Contracts.User;
global using SurveyBasket.Api.Authentication.Filters;
global using SurveyBasket.Api.Contracts.Polls;