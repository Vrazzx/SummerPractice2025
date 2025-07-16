namespace task13tests;

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task13;
using System.Text.Json;
public class JsonStudentServiceTests
{
    [Fact]
    public void Serialize_ValidStudent_ReturnsCorrectJson()
    {

        var student = new Student
        {
            FirstName = "Test",
            LastName = "User",
            BirthDate = new DateTime(2000, 1, 1)
        };


        var json = JsonStudentService.Serialize(student);


        Assert.Contains("\"FirstName\": \"Test\"", json);
        Assert.Contains("\"LastName\": \"User\"", json);
        Assert.Contains("\"BirthDate\": \"2000-01-01\"", json);
        Assert.DoesNotContain("\"Grades\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsStudent()
    {

        var json = "{\"FirstName\":\"Test\",\"LastName\":\"User\",\"BirthDate\":\"2000-01-01\"}";


        var student = JsonStudentService.Deserialize(json);


        Assert.Equal("Test", student.FirstName);
        Assert.Equal("User", student.LastName);
        Assert.Equal(new DateTime(2000, 1, 1), student.BirthDate);
        Assert.Null(student.Grades);
    }

    [Fact]
    public void Deserialize_InvalidDate_ThrowsException()
    {

        var json = "{\"FirstName\":\"Test\",\"LastName\":\"User\",\"BirthDate\":\"invalid-date\"}";


        Assert.Throws<JsonException>(() => JsonStudentService.Deserialize(json));
    }

    [Fact]
    public void SaveAndLoad_ValidStudent_ReturnsSameData()
    {

        var tempFile = Path.GetTempFileName();
        var student = new Student
        {
            FirstName = "Test",
            LastName = "User",
            BirthDate = new DateTime(2000, 1, 1),
            Grades = new List<Subject> { new Subject("Math", 5) }
        };

        try
        {

            JsonStudentService.SaveToFile(student, tempFile);
            var loadedStudent = JsonStudentService.LoadFromFile(tempFile);


            Assert.Equal(student.FirstName, loadedStudent.FirstName);
            Assert.Equal(student.LastName, loadedStudent.LastName);
            Assert.Equal(student.BirthDate, loadedStudent.BirthDate);
            Assert.Equal(student.Grades[0].Name, loadedStudent.Grades[0].Name);
            Assert.Equal(student.Grades[0].Grade, loadedStudent.Grades[0].Grade);
        }
        finally
        {

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void Deserialize_InvalidGrade_ThrowsException()
    {

        var json = "{\"FirstName\":\"Test\",\"LastName\":\"User\",\"BirthDate\":\"2000-01-01\"," +
                   "\"Grades\":[{\"Name\":\"Math\",\"Grade\":6}]}";


        var ex = Assert.Throws<JsonException>(() => JsonStudentService.Deserialize(json));
        Assert.Contains("Grade должно быть между 1 и 5", ex.Message);
    }
}
