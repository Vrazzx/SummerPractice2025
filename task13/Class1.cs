namespace task13;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;

public class Subject
{
    public string Name { get; set; }
    public int Grade { get; set; }

    public Subject() { }

    public Subject(string name, int grade)
    {
        Name = name;
        Grade = grade;
    }
}

public class Student
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Subject> Grades { get; set; }

    public Student() { }

    public Student(string firstName, string lastName, DateTime birthDate, List<Subject> grades = null)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Grades = grades;
    }
}
public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-mm-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return DateTime.ParseExact(reader.GetString(), Format, null);
        }
        catch (FormatException ex)
        {
            throw new JsonException("Неверный формат даты. Ожидается: yyyy-mm-dd", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
public static class JsonStudentService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonDateTimeConverter() }
    };

    public static string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, _options);
    }

    public static Student Deserialize(string json)
    {
        try
        {
            var student = JsonSerializer.Deserialize<Student>(json, _options);
            
           
            if (string.IsNullOrWhiteSpace(student.FirstName))
                throw new JsonException("FirstName не может быть пусто");
                
            if (string.IsNullOrWhiteSpace(student.LastName))
                throw new JsonException("LastName не может быть пусто");
                
            if (student.BirthDate > DateTime.Now)
                throw new JsonException("BirthDate не может быть больше текущей даты");
                
            if (student.Grades != null)
            {
                foreach (var grade in student.Grades)
                {
                    if (string.IsNullOrWhiteSpace(grade.Name))
                        throw new JsonException("Subject не может быть пусто");
                        
                    if (grade.Grade < 1 || grade.Grade > 5)
                        throw new JsonException("Grade должно быть между 1 и 5");
                }
            }
            
            return student;
        }
        catch (JsonException ex)
        {
            throw new JsonException("Ошибка десериализации Student: " + ex.Message, ex);
        }
    }

    public static void SaveToFile(Student student, string filePath)
    {
        var json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public static Student LoadFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
}