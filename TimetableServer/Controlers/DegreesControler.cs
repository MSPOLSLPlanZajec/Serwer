﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimetableServer.Models;

namespace TimetableServer.Controlers
{
    public class DegreeController : ApiController
    {
        public IEnumerable<Degree> GetAllDegrees()
        {
            return new List<Degree>()
            {
               new Degree() {id="1",title="dr" },
               new Degree() {id="1",title="prof dr hab." }
            };
        }
    }
}
