//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimetableServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class teacher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public teacher()
        {
            this.lessons = new HashSet<lesson>();
            this.teachings = new HashSet<teaching>();
        }
    
        public string idteachers { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string idtitles { get; set; }
        public string idclassrooms { get; set; }
    
        public virtual classroom classroom { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<lesson> lessons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<teaching> teachings { get; set; }
        public virtual title title { get; set; }
    }
}