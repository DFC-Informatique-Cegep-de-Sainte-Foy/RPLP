﻿using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotOrganisation : IDepotOrganisation
    {
        private readonly RPLPDbContext _context;

        public DepotOrganisation()
        {
            this._context = new RPLPDbContext();
        }
               
        public Organisation GetOrganisationById(int p_id)
        {
            Organisation organisation = this._context.Organisations.Where(organisation => organisation.Id == p_id).Select(organisation => organisation.ToEntity()).FirstOrDefault();

            if (organisation == null)
                return new Organisation();

            return organisation;
        }

        public List<Organisation> GetOrganisations()
        {
            return this._context.Organisations.Select(organisation => organisation.ToEntity()).ToList();
        }

        
        public void AddAdministratorToOrganisation(string p_organisationName, string p_adminUsername)
        {
            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Username == p_adminUsername).FirstOrDefault();

            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName).SingleOrDefault();

                if (organisationResult != null)
                {
                    organisationResult.Administrators.Add(adminResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();
                }
            }
        }

        
        public void AddClassroomToOrganisation(string p_organisationName, string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.ClassroomName == p_classroomName).FirstOrDefault();

            if (classroomResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName).SingleOrDefault();

                if (organisationResult != null)
                {
                    organisationResult.Classrooms.Add(classroomResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();
                }
            }
        }

        public void RemoveAdministratorFromOrganisation(string p_organisationName, string p_adminUsername)
        {
            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Username == p_adminUsername).FirstOrDefault();
                        
            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName).SingleOrDefault();

                if (organisationResult != null)
                {
                    organisationResult.Administrators.Remove(adminResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();
                }
            }
        }

        public void RemoveClassroomFromOrganisation(string p_organisationName, string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.ClassroomName == p_classroomName).FirstOrDefault();

            if (classroomResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName).SingleOrDefault();

                if (organisationResult != null)
                {
                    organisationResult.Classrooms.Remove(classroomResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();
                }
            }
        }

        public void UpsertOrganisation(Organisation p_organisation)
        {
            List<Administrator_SQLDTO> administrators = new List<Administrator_SQLDTO>();

            if (p_organisation.Administrators.Count >= 1)
            {
                foreach (Administrator administrator in p_organisation.Administrators)
                {
                    administrators.Add(new Administrator_SQLDTO(administrator));
                }
            }

            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Id == p_organisation.Id).FirstOrDefault();

            if (organisationResult != null)
            {
                organisationResult.Name = p_organisation.Name;
                organisationResult.Administrators = administrators;

                this._context.Update(organisationResult);
                this._context.SaveChanges();
            }
            else
            {
                Organisation_SQLDTO organisation = new Organisation_SQLDTO();
                organisation.Name = p_organisation.Name;
                organisation.Administrators = administrators;
                organisation.Active = true;

                this._context.Organisations.Add(organisation);
                this._context.SaveChanges();
            }
        }

        public void DeleteOrganisation(string p_organisationName)
        {
            Organisation_SQLDTO OrganisationResult = this._context.Organisations.FirstOrDefault(organisation => organisation.Name == p_organisationName);

            if (organisationResult != null)
            {
                organisationResult.Active = false;

                this._context.Update(organisationResult);
                this._context.SaveChanges();
            }
        }
    }
}
