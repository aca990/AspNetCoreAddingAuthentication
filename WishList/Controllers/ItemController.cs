﻿using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Data;
using WishList.Models;

namespace WishList.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var logedInUser = _userManager.GetUserAsync(HttpContext.User).Result;
            var model = _context.Items.Where(item => item.User.Id == logedInUser.Id).ToList();

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Item item)
        {
            item.User = _userManager.GetUserAsync(HttpContext.User).Result;
            _context.Items.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var logedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

            var item = _context.Items.FirstOrDefault(e => e.Id == id);
            if (item.User.Id == logedInUser.Id)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
