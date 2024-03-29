﻿using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES.InterfacesDepots
{
    public interface IDepotAdministrator
    {
        public List<Administrator> GetAdministrators();
        public List<Administrator> GetDeactivatedAdministrators();
        public Administrator GetAdministratorById(int p_id);
        public Administrator GetAdministratorByUsername(string p_adminUsername);
        public Administrator GetAdministratorByEmail(string p_email);
        public List<Organisation> GetAdminOrganisations(string p_adminUsername);
        public void UpsertAdministrator(Administrator p_administrator);
        public void JoinOrganisation(string p_adminUsername, string p_organisationName);
        public void LeaveOrganisation(string p_adminUsername, string p_organisationName);
        public void DeleteAdministrator(string p_adminUsername);
        public void ReactivateAdministrator(string p_adminUsername);
    }
}
