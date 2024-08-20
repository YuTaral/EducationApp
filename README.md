This is my master's thesis project for the Technical University of Sofia - Plovdiv branch(I graduated October 2023). The app is designed for remote education. The used technology is .NET 7.0. I have used the MVC architecture. Currently it supports only localhost. The app supports three types of users - Admin, Teacher and Student. Here is a brief description of their roles:

    Admin - the only and only purpose of the admin is to approve new accounts. The admin is being seed the with username "owner@edu.com" and password "systemowner"
    Teacher - the teacher have many privileges which include creation of courses and lesson each course. Each type of lesson (Lecture, Exercise and Test) supports file upload. The teacher can upload homework for the students and create exams. After student finishes the exam the teacher can review the answers and grade the exam. The teacher user is responsible for assigning courses for the students.
    Student - the student can review the courses which are assigned and all of it's lessons. File upload is supported when there is a homework created by the teacher. Tests are also available in the given timeframe, decided by the teacher.

The app was done back in the summer of 2023, so obviously it need some improvements.
