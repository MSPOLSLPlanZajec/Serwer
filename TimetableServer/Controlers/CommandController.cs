﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimetableServer.Models;
using System.Web.Http.Cors;

namespace TimetableServer.Controlers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CommandController : ApiController
    {
        private DataBase _db = new DataBase();

        // POST: Command
        [Authorize]
        public JObject Post([FromBody] Command value)
        {
            _db = _db ?? new DataBase();
            switch (value.type)
            {
                case "add_study_plan":
                    return addStudyPlan(value);
                case "add_classroom":
                    return addClassroom(value);
                case "add_teacher":
                    return addTeacher(value);
                case "select_start":
                    return selectStart(value);
                default:
                    return null;
            }
        }

        private JObject selectStart(Command value)
        {
            var lessonTDS = JsonConvert.DeserializeObject<LessonTimeAndDateSetter>(value.data.ToString());
            lesson lesson = _db.getLesson(lessonTDS.id);
            lesson.iddays = lessonTDS.day;

            lesson.start = lessonTDS.startsAt;
            lesson.idclassrooms = lessonTDS.classroom;
            _db.updateLesson(lessonTDS.id, lesson);
            return JObject.Parse(JsonConvert.SerializeObject(lessonTDS));
        }

        private JObject addTeacher(Command value)
        {
            var teacherObj = JsonConvert.DeserializeObject<Teacher>(value.data.ToString());
            teacher teacher;
            if (teacherObj.id != null)
            {
                teacher = _db.getTeacher(teacherObj.id);
                teacher.name = teacherObj.name;
                teacher.surname = teacherObj.surname;
                teacher.idtitles = teacherObj.title;
                _db.updateTeacher(teacherObj.id,teacher);

            }
            else
            {
                teacher = new teacher();
                teacher.name = teacherObj.name;
                teacher.surname = teacherObj.surname;
                teacher.idtitles = teacherObj.title;
                teacher.idteachers = Guid.NewGuid().ToString().Replace("-", "");
                _db.insertTeacher(ref teacher);
                teacherObj.id = teacher.idteachers;
            }
           
            return JObject.Parse(JsonConvert.SerializeObject(teacherObj));
        }

        private JObject addClassroom(Command value)
        {

            var classroomObj = JsonConvert.DeserializeObject<Classroom>(value.data.ToString());
            classroom classroom;
            if (classroomObj.id != null)
            {
                classroom = _db.getClassRoom(classroomObj.id);
                classroom.number = classroomObj.name;
                classroom.idcroomtype = classroomObj.typeId;
                _db.updateClassRoom(classroomObj.id,classroom);
                classroomObj.id = classroom.idclassrooms;
            }
            else
            {
                classroom = new classroom();
                classroom.number = classroomObj.name;
                classroom.idcroomtype = classroomObj.typeId;
                //TODO ewentualnie zwiększyć maksymalną długość w bazie
                classroom.idclassrooms = Guid.NewGuid().ToString().Replace("-", "");
                _db.insertClassRoom(ref classroom);
                classroomObj.id = classroom.idclassrooms;
            }
            return JObject.Parse(JsonConvert.SerializeObject(classroomObj));
        }

        private JObject addStudyPlan(Command value)
        {
            var studyPlanObj = JsonConvert.DeserializeObject<StudyPlan>(value.data.ToString());
            var major = new faculty();
            major.name = studyPlanObj.major;
            major.idfaculty = Guid.NewGuid().ToString().Substring(0, 32).Replace("-", "");
            _db.insertFaculty(ref major);
            

            group group = new group();
            group.idgroups = Guid.NewGuid().ToString().Replace("-", "");
            group.name = studyPlanObj.major;
            group.idsemesters = null;
            group.idsupergroup = null;
            group.idfaculty = major.idfaculty;

            studyPlanObj.id = group.idgroups;

            _db.insertGroup(ref group);

            foreach (SubGroup it in studyPlanObj.semesters)
            {
                semester semester = new semester();
                semester.idsemesters = Guid.NewGuid().ToString().Replace("-", "");
                semester.name = it.name;
                _db.insertSemester(ref semester);
                addGroup(it, semester.idsemesters, group.idgroups, major.idfaculty);
            }

            return JObject.Parse(JsonConvert.SerializeObject(studyPlanObj));
        }

        private void addGroup(SubGroup subgroup, string semesterID, string superGroupID, string majorID)
        {
            group group = new group();
            group.idgroups = Guid.NewGuid().ToString().Replace("-", "");
            group.name = subgroup.name;
            group.idsemesters = semesterID;
            group.idsupergroup = superGroupID;
            group.idfaculty = majorID;
            try
            {
                _db.insertGroup(ref group);
            }
            catch (DbEntityValidationException e)
            {
                Console.WriteLine(e);
            }
            foreach (SubGroup it in subgroup.subgroups)
            {
                addGroup(it, semesterID, group.idgroups, majorID);
            }
            foreach (Lesson it in subgroup.subjects)
            {
                addSubject(it, group.idgroups);
            }
            subgroup.id = group.idgroups;
        }

        private void addSubject(Lesson it, string groupID)
        {
            lesson lesson = new lesson();
            lesson.idlessons = Guid.NewGuid().ToString().Replace("-", "");
            lesson.idteachers = it.teacher_id;
            lesson.idsubjects = findSubjectID(it.name, it.type, it.duration);
            lesson.idgroups = groupID;
            try
            {
                _db.insertLesson(ref lesson);
            }
            catch (DbEntityValidationException e)
            {
                Console.WriteLine(e);
            }
        }

        private string findSubjectID(string name, string type, int? duration)
        {
            var idType = _db.getAllCRTypes().First(crt => crt.name == type).idcroomtype;
            foreach (var it in _db.getAllSubjects())
            {
                if (it.name == name && it.time == duration && it.type == idType)
                    return it.idsubjects;
            }
            subject subject = new subject();
            subject.idsubjects = Guid.NewGuid().ToString().Replace("-", "");
            subject.name = name;
            subject.time = duration;
            subject.type = idType;
            _db.insertSubject(ref subject);
            return subject.idsubjects;
        }
    }
}
