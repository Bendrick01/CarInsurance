using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurees insurees = db.Insurees.Find(id);
            if (insurees == null)
            {
                return HttpNotFound();
            }
            return View(insurees);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insurees insurees)
        {
            //Assignment Code

            //If statement to check if the form submission is valid (If ModelState.IsValid) and if so, add to the database and redirect to QuoteConfirm
            //If not valid then returns the create view
            //If true then Calculate the quote using the CalculateQuote method below and assigning to an instance of insurees class
            //Add the details to the db
            //Save the changes
            if (ModelState.IsValid)
            {
                CalculateQuote(insurees); 
                db.Insurees.Add(insurees);
                db.SaveChanges();

                var viewModel = new QuoteConfirm { QuoteAmount = insurees.Quote };
                return View("QuoteConfirm", viewModel); // Redirect to a new QuoteConfirm View
            }
            return View(insurees);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurees insurees = db.Insurees.Find(id);
            if (insurees == null)
            {
                return HttpNotFound();
            }
            return View(insurees);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insurees insurees)
        {
            //Assignment Code

            //If statement to check if the form submission is valid (If ModelState.IsValid)
            //If valid then calls the CalculateQuote method. If not valid then returns the create view
            //State that the insurees object has been modified
            //Save the modified object the database
            //Returns the users to index page
            if (ModelState.IsValid)
            {
                CalculateQuote(insurees);
                db.Entry(insurees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insurees);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurees insurees = db.Insurees.Find(id);
            if (insurees == null)
            {
                return HttpNotFound();
            }
            return View(insurees);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insurees insurees = db.Insurees.Find(id);
            db.Insurees.Remove(insurees);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Assingment Code
        private void CalculateQuote(Insurees insuree)
        {
            //Setting the base at 50

            //Assigning the value of 50 to the variable baseQuote
            //Using m suffix to ensure the number is treated as a decimal
            decimal baseQuote = 50m;

            //Calculate the age of the driver
            //Assinging todays date using DateTime.Today to the variable today
            //Working out a base age by subtracting the birth year from the current year
            //Checking if the insuree has had a birthday yet in the current year
            //If Insuree date of birth is after todays date then use the AddYears method to minus one year from the age variable
            var today = DateTime.Today;
            var age = today.Year - insuree.DateOfBirth.Year;
            if (insuree.DateOfBirth.AddYears(age) > today) age--;

            //If statement to add money based on the age of the driver
            //If the driver is 18 years old or less then add 100 to the baseQuote variable
            //Else if the driver is between 19 and 25 then add 50 to the baseQuote variable
            //Any other age add 25 to the baseQuote variable
            if (age <= 18) baseQuote += 100;
            else if (age >= 19 && age <= 25) baseQuote += 50;
            else baseQuote += 25;

            //Adjusting the baseQuote variable based on the year of manufacture
            //If the Car is manufactured before 2000 or after 2015 then add 25 to the baseQuote variable
            if (insuree.CarYear < 2000 || insuree.CarYear > 2015) baseQuote += 25;

            //Adjusting the baseQuote variable based in the Make and Model of the car
            //If the car make is a Porsche then add 25 to the baseQuote variable
            //If the car model is 911 carrera then add 25 to the baseQuote variable
            if (insuree.CarMake.ToLower() == "porsche")
            {
                baseQuote += 25;
                if (insuree.CarModel.ToLower() == "911 carrera") baseQuote += 25;
            }

            //Adjusting the baseQuote variable based on number of speeding tickets entered
            //Multiplies the number entered by 10 and adds the result to the baseQuote variable
            baseQuote += (insuree.SpeedingTickets * 10);

            //Adjusting the baseQuote variable based on whether DUI checkbox is ticked
            //If the checkbox is true then multiply the baseQuote variable value by 1.25
            if (insuree.DUI) baseQuote *= 1.25m;

            //Adjusting the baseQuote variable based on whether coverage checkbox is ticked
            //If the checkbox is true then multiply the baseQuote variable value by 1.5
            if (insuree.CoverageType) baseQuote *= 1.5m;

            //Returning the final value of the baseQuote variable to insuree.Quote
            insuree.Quote = baseQuote;
        }

        public ActionResult Admin()
        {
            var insurees = db.Insurees.ToList();
            return View(insurees);
        }
    }
}
