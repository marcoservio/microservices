﻿using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Utils;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;
public class ProductController : Controller
{
    private readonly IProductService _service;

    public ProductController(IProductService productService)
    {
        _service = productService;
    }

    public async Task<IActionResult> ProductIndex()
    {
        var products = await _service.FindAll("");
        return View(products);
    }

    public IActionResult ProductCreate()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ProductCreate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _service.Create(model, token!);

            if (response != null)
                return RedirectToAction(nameof(ProductIndex));
        }

        return View(model);
    }

    public async Task<IActionResult> ProductUpdate(int id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var model = await _service.FindById(id, token!);

        if (model != null)
            return View(model);

        return NotFound();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ProductUpdate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _service.Update(model, token!);

            if (response != null)
                return RedirectToAction(nameof(ProductIndex));
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> ProductDelete(int id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var model = await _service.FindById(id, token!);

        if (model != null)
            return View(model);

        return NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> ProductDelete(ProductViewModel model)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var response = await _service.DeleteById(model.Id, token!);

        if (response)
            return RedirectToAction(nameof(ProductIndex));

        return View(model);
    }
}
