using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using PartyPlanner.Models;

namespace PartyPlanner.Controllers
{
    public class PartyPlannerController : ApiController
    {
        private PartyPlannerContext db = new PartyPlannerContext();

        // GET api/PartyPlanner
        public IEnumerable<Party> GetParties()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Parties.Include(g => g.Guests)
                                .Include(s => s.ShoppingList)
                                .AsEnumerable();
        }

        // GET api/PartyPlanner/5
        public Party GetParty(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Parties.Include("ShoppingList");
            db.Parties.Include("Guests");
            Party party = db.Parties.Find(id);
            db.Entry(party).Collection(u => u.ShoppingList).Load();
            if (party == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return party;
        }

        // PUT api/PartyPlanner/5
public HttpResponseMessage PutParty(int id, Party party)
{
    if (ModelState.IsValid )
    {               
        db.Entry(party).State = EntityState.Modified;

        if (party.ShoppingList != null)
        {
            foreach (var item in party.ShoppingList)
            {
                if (item.ShoppingItemID == 0)
                {
                    item.Party = party;
                    db.Entry(item).State = EntityState.Added;
                }
                else
                {
                    db.Entry(item).State = EntityState.Modified;
                }
            }
        }

        if (party.Guests != null)
        {
            foreach (var item in party.Guests)
            {
                if (item.GuestID == 0)
                {
                    item.Party = party;
                    db.Entry(item).State = EntityState.Added;
                }
                else
                {
                    db.Entry(item).State = EntityState.Modified;
                }
            }
        }                
        try
        {
            db.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, party);
        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = party.PartyID }));
        return response;
    }
    else
    {
        return Request.CreateResponse(HttpStatusCode.BadRequest);
    }
}

        // POST http://localhost:21962/api/PartyPlanner
        public HttpResponseMessage PostParty(Party party)
        {
            if (ModelState.IsValid)
            {
                if (party.PartyID == 0)
                    db.Parties.Add(party);
                else
                {
                    db.Entry(party).State = EntityState.Modified;
                }
                db.SaveChanges();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, party);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = party.PartyID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        

        // DELETE api/PartyPlanner/5
        public HttpResponseMessage DeleteParty(int id)
        {

            Party party = db.Parties.Find(id);
            if (party == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Parties.Remove(party);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, party);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}