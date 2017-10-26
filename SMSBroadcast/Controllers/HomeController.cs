using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMSBroadcast.Models;
using System.IO;
using SMSBroadcast.DAL;
using System.Text.RegularExpressions;

namespace SMSBroadcast.Controllers
{
    public class HomeController : Controller
    {
        BroadcastContext db = new BroadcastContext();
        [HttpGet]
        public ActionResult Index()
        {
            var models = db.Broadcasts.Select(x => new BroadcastViewModel { Id = x.Id, GroupId = x.GroupId, Message = x.Messsage, SendDate = x.BroadcastDateTime,Group=x.Group }).ToList();
            models.ForEach(x => x.SendTime = x.SendDate.TimeOfDay);
            var viewModels = new BroadcastViewModel()
            {
                Messages = models,
            };
            return View(viewModels);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var models = db.Broadcasts.Select(x => new BroadcastViewModel { Id = x.Id, GroupId = x.GroupId, Message = x.Messsage, SendDate = x.BroadcastDateTime, Group = x.Group }).ToList();
            models.ForEach(x => x.SendTime = x.SendDate.TimeOfDay);
            var model = models.FirstOrDefault(x => x.Id == id);
            model.Messages = models;
            return View("Index", model);
        }

        
        [HttpPost]
        public ActionResult Create(BroadcastViewModel model)
        {
            var entity = db.Broadcasts.FirstOrDefault(x => x.Messsage == model.Message);
            if(entity!=null&&entity.Id!=model.Id)
            {
                var date = entity.BroadcastDateTime - model.SendDate.Add(model.SendTime);
                if(date.Days<1)
                {
                    ModelState.AddModelError("Message", "Message is already submitted for this day");
                }
            }

            if (string.IsNullOrEmpty(model.Group.Name))
            {
                ModelState.AddModelError("Group.Name", "Group name is required");
            }
            if (string.IsNullOrEmpty(model.Message))
            {
                ModelState.AddModelError("Message", "Message is required");
            }
            else
            {
                Regex regex = new Regex("[@€$&#%]+");
                if (regex.Match(model.Message).Length > 0)
                {
                    ModelState.AddModelError("Message", "@ € $ & # % characters not allowed");
                }
            }
            if (model.SendDate == null)
            {
                ModelState.AddModelError("SendDate", "Send date is required");
            }
            if (model.SendTime == null)
            {
                ModelState.AddModelError("SendTime", "Send time is required");
            }

            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index", model);
            }

            if (model.Id == 0)
            {
                if (model.FileUpload == null)
                {
                    ModelState.AddModelError("FileUpload", "File is required");
                    return RedirectToAction("Index", model);
                }
                

                var filePath = FileUpload(model.FileUpload);
                

                var contacts = GetContacts(filePath);

                db.Groups.Add(model.Group);
                db.SaveChanges();
                foreach (var contact in contacts)
                {
                    contact.GroupId = model.Group.Id;
                    db.Contacts.Add(contact);
                }
                var broadcastEntity = new Broadcast
                {
                    GroupId = model.Group.Id,
                    Messsage = model.Message,
                    BroadcastDateTime = model.SendDate.Add(model.SendTime)
                };

                db.Broadcasts.Add(broadcastEntity);
                db.SaveChanges();
            }
            else
            {
                

                if (model.FileUpload != null)
                {
                    var filePath = FileUpload(model.FileUpload);
                    var contacts = GetContacts(filePath);

                    db.Contacts.RemoveRange(model.Group.Contacts);
                    db.SaveChanges();

                    foreach (var contact in contacts)
                    {
                        contact.GroupId = model.GroupId;
                        db.Contacts.Add(contact);
                    }
                }

                var group = db.Groups.FirstOrDefault(x => x.Id == model.GroupId);
                group.Name = model.Group.Name;
                var broadcast = db.Broadcasts.FirstOrDefault(x => x.Id == model.Id);
                broadcast.Messsage = model.Message;
                broadcast.BroadcastDateTime = model.SendDate.Add(model.SendTime);
                db.SaveChanges();
            }

            var models = db.Broadcasts.Select(x => new BroadcastViewModel { Id = x.Id, GroupId = x.GroupId, Message = x.Messsage, SendDate = x.BroadcastDateTime, Group=x.Group}).ToList();
            models.ForEach(x => x.SendTime = x.SendDate.TimeOfDay);
            var viewModels = new BroadcastViewModel()
            {
                Messages = models,
            };
            return RedirectToAction("Index", viewModels);
        }

       
        public ActionResult Delete(int? id)
        {
            var model = db.Broadcasts.FirstOrDefault(x => x.Id == id);
            db.Contacts.RemoveRange(model.Group.Contacts);
            db.Groups.Remove(model.Group);
            db.Broadcasts.Remove(model);
            db.SaveChanges();

            var models = db.Broadcasts.Select(x => new BroadcastViewModel { Id = x.Id, GroupId = x.GroupId, Message = x.Messsage, SendDate = x.BroadcastDateTime, Group = x.Group }).ToList();
            models.ForEach(x => x.SendTime = x.SendDate.TimeOfDay);
            var viewModels = new BroadcastViewModel()
            {
                Messages = models,
            };
            return RedirectToAction("Index", model);
        }



        private List<Contact> GetContacts(string filePath)
        {
            List<Contact> contacts = new List<Contact>();
            string extension = Path.GetExtension(filePath);

            if (!string.IsNullOrEmpty(filePath))
            {
                
                string csvData = System.IO.File.ReadAllText(filePath);

                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        contacts.Add(new Contact
                        {
                            Phone = row.Split(',')[0]
                        });
                    }
                }
            }
            return contacts;
        }

        private string FileUpload(HttpPostedFileBase file)
        {
            List<ContactViewModel> contacts = new List<ContactViewModel>();

            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))

            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = path + Path.GetFileName(file.FileName);
               
                file.SaveAs(filePath);
                return filePath;
            }

            return string.Empty;
        }

    }
    
}