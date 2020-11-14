using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using BookStoreMVC.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Administrator)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
           // A API(see API Region below) is called to populate the table.
            return View();
        }


        public IActionResult Upsert(int? id)
        {
            var coverType = new CoverType();
            if (id == null) // No ID provided
            {
                return View(coverType);
            }

            //coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault()); // This uses EF core to handle the operation.
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id); // We use this to map the ID like defined in the stored procedure made with migration 
            coverType = _unitOfWork.StoredProcedure_Call.OnceRecord<CoverType>(SD.Proc_CoverType_Get, parameter); ;

            if (coverType == null) // ID provided dose not match with any ID in DB
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {

                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);

                if(coverType.Id == 0)
                {
                    //_unitOfWork.CoverType.Add(coverType);
                    _unitOfWork.StoredProcedure_Call.Execute(SD.Proc_CoverType_Create, parameter);
                }
                else
                {
                    //_unitOfWork.CoverType.Update(coverType);
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.StoredProcedure_Call.Execute(SD.Proc_CoverType_Update, parameter);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        #region API CAllS
        
        [HttpGet]
        public IActionResult GetAll()
        {
            //var allCoverTypes = _unitOfWork.CoverType.GetAll(); // This uses EF core to handle the operation.
            var allCoverTypes = _unitOfWork.StoredProcedure_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = allCoverTypes });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //var coverType = _unitOfWork.CoverType.Get(id); // This uses EF core to handle the operation.
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id); // We use this to map the ID like defined in the stored procedure made with migration 
            var coverType = _unitOfWork.StoredProcedure_Call.OnceRecord<CoverType>(SD.Proc_CoverType_Get, parameter); 

            if (coverType == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            //_unitOfWork.CoverType.Remove(coverType); // This uses EF core to handle the operation.
            _unitOfWork.StoredProcedure_Call.Execute(SD.Proc_CoverType_Delete, parameter);
            _unitOfWork.Save();

            return Json(new { success = true, message = $"{coverType.Name} was deleted" });
        }

        #endregion
    }
}
