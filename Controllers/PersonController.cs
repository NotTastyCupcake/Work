﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Work.Models;

namespace Work.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonContext _context;

        public PersonController(PersonContext context)
        {
            _context = context;
        }

        // GET: Person
        public async Task<IActionResult> Index()
        {
            var personContext = _context.Persons.Include(m => m.Department).Include(m => m.Language);
            return View(await personContext.ToListAsync());
        }


        // GET: Person/Create
        public IActionResult Add()
        {
            PopulateDepartmentsDropDownList();
            PopulateLanguageDropDownList();
            return View();
        }

        // POST: Person/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("IdPerson,FirstName,LastName,Age,Gender,IdDepartment,IdLanguage")] MainPersonModel mainPersonModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mainPersonModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(mainPersonModel.IdDepartment);
            PopulateLanguageDropDownList(mainPersonModel.IdLanguage);
            return View(mainPersonModel);
        }

        // GET: Person/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainPersonModel = await _context.Persons.FindAsync(id);
            if (mainPersonModel == null)
            {
                return NotFound();
            }
            PopulateDepartmentsDropDownList(mainPersonModel.IdDepartment);
            PopulateLanguageDropDownList(mainPersonModel.IdLanguage);
            return View(mainPersonModel);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPerson,FirstName,LastName,Age,Gender,IdDepartment,IdLanguage")] MainPersonModel mainPersonModel)
        {
            if (id != mainPersonModel.IdPerson)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mainPersonModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MainPersonModelExists(mainPersonModel.IdPerson))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(mainPersonModel.IdDepartment);
            PopulateLanguageDropDownList(mainPersonModel.IdLanguage);
            return View(mainPersonModel);
        }

        // GET: Person/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainPersonModel = await _context.Persons
                .Include(m => m.Department)
                .Include(m => m.Language)
                .FirstOrDefaultAsync(m => m.IdPerson == id);
            if (mainPersonModel == null)
            {
                return NotFound();
            }

            return View(mainPersonModel);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mainPersonModel = await _context.Persons.FindAsync(id);
            _context.Persons.Remove(mainPersonModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Deportaments
                                   orderby d.NameDepart
                                   select d;
            ViewBag.IdDepartment = new SelectList(departmentsQuery, "IdDepartment", "NameDepart", selectedDepartment);
        }

        private void PopulateLanguageDropDownList(object selectedLanguages = null)
        {
            var languageQuery = from d in _context.Languages
                                orderby d.NameLanguage
                                select d;
            ViewBag.IdLanguage = new SelectList(languageQuery, "IdLanguage", "NameLanguage", selectedLanguages);
        }

        private bool MainPersonModelExists(int id)
        {
            return _context.Persons.Any(e => e.IdPerson == id);
        }
    }
}
