using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA_DAL.Models;
using OA_Repository.Identity;
using OA_Service.Bases;
using OA_Service.Interfaces;
using OA_Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA_Service.AppServices
{
    public class ApplicantAppService: BaseAppService<Applicant>
    {
        public ApplicantAppService(IUnitOfWork _unit) : base(_unit)
        {
        }

        public List<ApplicantViewModel> GetAllApplicants()
        {
            return Mapper.Map<List<ApplicantViewModel>>(TheUnitOfWork.Applicant.GetAllApplicants());
        }
        public List<ApplicantViewModel> GetAllApplicantsThatHired()
        {
            return Mapper.Map<List<ApplicantViewModel>>(TheUnitOfWork.Applicant.GetApplicantThatHired());
        }

        public ApplicantViewModel GetById(int id)
        {
            return Mapper.Map<ApplicantViewModel>(TheUnitOfWork.Applicant.GetApplicantById(id));
        }

        public Applicant GetByModelId(int id)
        {
            return TheUnitOfWork.Applicant.GetApplicantById(id);
        }


        public bool SaveNewApplicant(ApplicantViewModel ApplicantViewModel)
        {
            bool result = false;
            var Applicant = Mapper.Map<Applicant>(ApplicantViewModel);
            if (TheUnitOfWork.Applicant.InsertApplicant(Applicant))
            {
                result = TheUnitOfWork.Commit() > new int();
            }
            return result;
        }

        public bool UpdateApplicant(ApplicantViewModel ApplicantViewModel)
        {
            var Applicant = Mapper.Map<Applicant>(ApplicantViewModel);
            TheUnitOfWork.Applicant.UpdateApplicant(Applicant);
            TheUnitOfWork.Commit();

            return true;
        }
        public bool ChangeHiredStatus(Applicant Applicant)
        {
            //var Applicant = Mapper.Map<Applicant>(ApplicantViewModel);
            TheUnitOfWork.Applicant.MakeApplicantHiredorUnhired(Applicant);
            TheUnitOfWork.Commit();

            return true;
        }

        public bool UpdateApplicantMyModel(Applicant Applicant)
        { 
            TheUnitOfWork.Applicant.UpdateApplicant(Applicant);
            TheUnitOfWork.Commit();
            return true;
        }
        public bool DeleteApplicant(int id)
        {
            TheUnitOfWork.Applicant.DeleteApplicant(id);
            bool result = TheUnitOfWork.Commit() > new int();
            return result;
        }

        public bool CheckApplicantExists(ApplicantViewModel ApplicantViewModel)
        {
            Applicant Applicant = Mapper.Map<Applicant>(ApplicantViewModel);
            return TheUnitOfWork.Applicant.CheckApplicantExists(Applicant);
        }
    }
}
