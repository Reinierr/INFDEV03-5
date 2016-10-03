using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_1_INFDEV03_5
{
  public class Headquarters
  {
    public string hqName { get; set; }
    public int rooms { get; set; }
    public double rent { get; set; }
    public string country { get; set; }
    public string postalcode { get; set; }
  }

  public class Employee
  {
    public int employeeId { get; set; }
    public string hqName { get; set; }
    //public int employeeAid { get; set; }
    public int bsn { get; set; }
    public string name { get; set; }
    public string surname { get; set; }
  }

  public class Degree
  {
    public int degreeId { get; set; }
    //public int employeeId { get; set; }
    public string course { get; set; }
    public string school { get; set; }
    public string level { get; set; }
  }

  public class Address
  {
    public string country { get; set; }
    public string postalcode { get; set; }
    public string city { get; set; }
    public string street { get; set; }
    public string num { get; set; }
  }

  public class Position
  {
    public int posId { get; set; }
    public string posName { get; set; }
    public string description { get; set; }
    public double hour_fee { get; set; }
    public int projectId { get; set; }
  }

  public class Project
  {
    public int projectId { get; set; }
    public string hqName { get; set; }
    public double budget { get; set; }
    public int hours { get; set; }
    public bool payRent { get; set; }
  }

  public class Item
  {
    public string Value { get; set; }
    public string Text { get; set; }
  }
}
