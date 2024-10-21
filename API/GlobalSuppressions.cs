// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
    Justification = "<Pending>",
    Scope = "member",
    Target = "~M:API.Services.ItemService.SearchItems(System.String,System.Int32,System.Int32)~System.Threading.Tasks.Task{System.ValueTuple{API.DTOs.ItemResponseDto[],System.Int32}}"
)]
[assembly: SuppressMessage(
    "Performance",
    "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
    Justification = "<Pending>",
    Scope = "member",
    Target = "~M:API.Services.ItemService.SearchItems(System.String,System.Int32,System.Int32)~System.ValueTuple{System.Linq.IQueryable,System.Int32}"
)]
[assembly: SuppressMessage(
    "Performance",
    "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
    Justification = "<Pending>",
    Scope = "member",
    Target = "~M:API.Services.ItemService.SearchItems(System.String,System.Int32,System.Int32)~System.ValueTuple{System.Linq.IQueryable{API.Models.ItemModel},System.Int32}"
)]
