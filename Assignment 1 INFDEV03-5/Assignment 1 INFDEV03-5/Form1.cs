using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment_1_INFDEV03_5
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      preLoad();
    }

    // Selected objects
    public Address selectedAddress { get; set; }
    public Degree selectedDegree { get; set; }
    public Employee selectedEmployee_ { get; set; }
    public Position selectedPosition { get; set; }
    public Project selectedProject { get; set; }
    // End selected objects

    private void preLoad()
    {
      // Initialize form
      // Dropdown box for headquarters in new employee
      fillComboBoxHeadquarters(comboBox1);
      // End dropdown box for headquarters in new employee

      // Dropdown box for positions in new employee
      fillComboBoxPositions(comboBox2);
      // End dropdown box for positions in new employee

      // Dropdown box for headquarters in employee view
      fillComboBoxHeadquarters(comboBox3);
      // End dropdown box for headquarters in employee view

      // Dropdown box for positions in employee view
      fillComboBoxPositions(comboBox4);
      // End dropdown box for positions in employee view

      // Dropdown box for projects in employee view
      fillComboBoxProjects(comboBox5);
      // End dropdown box for projects in employee view

      // Dropdown box for headquarters in project view
      fillComboBoxHeadquarters(comboBox6);
      fillComboBoxHeadquarters(comboBox7);
      // End dropdown box for headquarters in project view

      reloadEmployees();
      reloadProjects();
    }

    private void reloadEmployees()
    {
      // Get all employees
      DBConnection newC = new DBConnection();
      DataGridView eGrid = dataGridView1;
      eGrid.AllowUserToAddRows = false;
      eGrid.ReadOnly = true;
      eGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      var employees = newC.Select("SELECT * FROM employees");
      List<Employee> eList = new List<Employee>();
      foreach (List<string> entry in employees)
      {
        Employee e = new Employee();
        e.employeeId = Convert.ToInt32(entry[0]);
        e.hqName = entry[1];
        //e.employeeAid = Convert.ToInt32(entry[2]);
        e.bsn = Convert.ToInt32(entry[3]);
        e.name = entry[4];
        e.surname = entry[5];
        eList.Add(e);
      }
      BindingList<Employee> eBindingList = new BindingList<Employee>(eList);
      BindingSource eSource = new BindingSource(eBindingList, null);
      eGrid.DataSource = eSource;
      // End all employees
    }

    private void reloadProjects()
    {
      // Get all employees
      DBConnection newC = new DBConnection();
      DataGridView prGrid = dataGridView2;
      prGrid.AllowUserToAddRows = false;
      prGrid.ReadOnly = true;
      prGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      var projects = newC.Select("select *, projectId as pId, "+
                                 "(select projectId from projects "+
                                  "INNER JOIN headquarters on projects.hqName = headquarters.hqname "+
                                  "where projects.budget/10 < headquarters.rent "+
                                  "and projects.projectId = pId) as rentpId "+
                                 "from projects");
      List<Project> prList = new List<Project>();
      foreach (List<string> entry in projects)
      {
        Project pr = new Project();
        pr.projectId = Convert.ToInt32(entry[0]);
        pr.hqName = entry[1];
        pr.budget = Convert.ToDouble(entry[2]);
        pr.hours = Convert.ToInt32(entry[3]);
        if(entry[0] == entry[6])
        {
          pr.payRent = false;
        }
        else
        {
          pr.payRent = true;
        }
        prList.Add(pr);
      }
      BindingList<Project> prBindingList = new BindingList<Project>(prList);
      BindingSource prSource = new BindingSource(prBindingList, null);
      prGrid.DataSource = prSource;
      // End all employees
    }

    private void button1_Click(object sender, EventArgs e)
    {
      // Save new employee
      DBConnection newC = new DBConnection();
      int bsn = Int32.Parse(BSN.Text);
      string name = eName.Text;
      string surname = Surname.Text;
      string country = Country.Text;
      string postalcode = Postalcode.Text;
      string city = City.Text;
      string street = Street.Text;
      string number = Number.Text;
      string hqName = Convert.ToString(comboBox1.SelectedValue);
      int position = Convert.ToInt32(comboBox2.SelectedValue);
      List<Degree> degrees = new List<Degree>();
      if(Course1.Text.Length > 0 && School1.Text.Length > 0 && Level1.Text.Length > 0)
      {
        Degree d1 = new Degree();
        d1.course = Course1.Text;
        d1.school = School1.Text;
        d1.level = Level1.Text;
        degrees.Add(d1);
      }
      if(Course2.Text.Length > 0 && School2.Text.Length > 0 && Level2.Text.Length > 0)
      {
        Degree d2 = new Degree();
        d2.course = Course2.Text;
        d2.school = School2.Text;
        d2.level = Level2.Text;
        degrees.Add(d2);
      }
      if(Course3.Text.Length > 0 && School3.Text.Length > 0 && Level3.Text.Length > 0)
      {
        Degree d3 = new Degree();
        d3.course = Course3.Text;
        d3.school = School3.Text;
        d3.level = Level3.Text;
        degrees.Add(d3);
      }
      newC.Execute("INSERT INTO addresses (country, postalcode, city, street, num)"+
                    "VALUES ('" + country + "','" + postalcode + "','" + city + "','" + street + "','" + number + "')");
      int pk = newC.ExecutePK("INSERT INTO employees (hqName, bsn, name, surname)"+
                    "VALUES('" + hqName + "', '" + bsn + "', '" + name + "', '" + surname + "'); select last_insert_id()");
      int adrId = newC.ExecutePK("INSERT INTO eAddresses (employeeId, country, postalcode)"+
                    "VALUES('" + pk + "', '" + country + "', '" + postalcode + "'); select last_insert_id()");
      newC.Execute("UPDATE employees SET employeeAid = '" + adrId + "' WHERE employeeId = '" + pk + "'");
      newC.Execute("INSERT INTO ePositions (posId, employeeId)"+
                    "VALUES('" + position + "', '" + pk + "')");
      foreach(Degree d in degrees)
      {
        newC.Execute("INSERT INTO degrees (employeeId, course, school, level)"+
                      "VALUES ('" + pk + "', '" + d.course + "', '" + d.school + "', '" + d.level + "')");
      }
      reloadEmployees();

      // Empty used form
      BSN.Text = ""; eName.Text = ""; Surname.Text = ""; Country.Text = ""; Postalcode.Text = ""; City.Text = ""; Street.Text = "";
      Number.Text = ""; Course1.Text = ""; Course2.Text = ""; Course3.Text = ""; School1.Text = ""; School2.Text = ""; School3.Text = "";
      Level1.Text = ""; Level2.Text = ""; Level3.Text = "";
      panel1.Visible = true; panel2.Visible = true;
      // End empty used form

      // End save employee
    }

    private void button2_Click(object sender, EventArgs e)
    {
      preLoad();
    }

    private void button3_Click(object sender, EventArgs e)
    {
      button3.Text = "Less";
      if(panel1.Visible == true && panel2.Visible == true)
      {
        panel1.Visible = false;
      }
      else
      {
        panel1.Visible = true;
        button3.Text = "More";
      }
    }

    private void button4_Click(object sender, EventArgs e)
    {
      button4.Text = "Less";
      if (panel2.Visible == true)
      {
        panel2.Visible = false;
      }
      else
      {
        panel2.Visible = true;
        button4.Text = "More";
      }
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      int employeeId = 0;
      panel3.Visible = false;
      //Select row from datagridview1
      if (dataGridView1.SelectedCells.Count > 0)
      {
        int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
        DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
        employeeId = Convert.ToInt32(selectedRow.Cells["employeeId"].Value);
      }
      //--Employee selected--

      //Get degrees that belong to employeeId
      DataGridView dGrid = eDegrees;
      dGrid.AllowUserToAddRows = false;
      dGrid.ReadOnly = true;
      dGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      fillDegrees(employeeId, dGrid);
      //End of employee degrees

      //Get addresses that belong to employeeId
      DataGridView aGrid = eAddresses;
      aGrid.AllowUserToAddRows = false;
      aGrid.ReadOnly = true;
      aGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      fillAddresses(employeeId, aGrid);
      //End of employee addresses

      //Get employee positions
      DataGridView pGrid = ePositions;
      pGrid.AllowUserToAddRows = false;
      pGrid.ReadOnly = true;
      pGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      fillPositions(employeeId, pGrid);
      //End of employee positions

      //Get employee information
      DBConnection newC = new DBConnection();
      var employee = newC.Select("SELECT * FROM employees WHERE employeeId = '" + employeeId + "'");
      Employee emp = new Employee();
      foreach (List<string> entry in employee)
      {
        emp.employeeId = Convert.ToInt32(entry[0]);
        emp.hqName = entry[1];
        emp.bsn = Convert.ToInt32(entry[3]);
        emp.name = entry[4];
        emp.surname = entry[5];
      }
      selectedEmployee.Text = emp.name+" "+emp.surname;
      edBSN.Text = Convert.ToString(emp.bsn);
      edName.Text = emp.name;
      edSurname.Text = emp.surname;
      selectedEmployee_ = emp;
      //Reset address/Degree
      selectedDegree = new Degree();
      selectedAddress = new Address();
      //End of employee information
    }
    
    private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      panel4.Visible = false;
      Project pr = new Project();
      if (dataGridView2.SelectedCells.Count > 0)
      {
        int selectedrowindex = dataGridView2.SelectedCells[0].RowIndex;
        DataGridViewRow selectedRow = dataGridView2.Rows[selectedrowindex];
        pr.projectId = Convert.ToInt32(selectedRow.Cells["projectId"].Value);
        pr.hqName = Convert.ToString(selectedRow.Cells["hqName"].Value);
        pr.budget = Convert.ToDouble(selectedRow.Cells["budget"].Value);
        pr.hours = Convert.ToInt32(selectedRow.Cells["hours"].Value);
      }
      comboBox7.SelectedIndex = comboBox7.FindStringExact(Convert.ToString(pr.hqName));
      ePbudget.Text = Convert.ToString(pr.budget);
      ePhours.Text = Convert.ToString(pr.hours);
      selectedProject = pr;
      sProject.Text = "Deselect";
    }

    private void eDegrees_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      Degree d = new Degree();
      if (eDegrees.SelectedCells.Count > 0)
      {
        int selectedrowindex = eDegrees.SelectedCells[0].RowIndex;
        DataGridViewRow selectedRow = eDegrees.Rows[selectedrowindex];
        d.degreeId = Convert.ToInt32(selectedRow.Cells["degreeId"].Value);
        d.course = Convert.ToString(selectedRow.Cells["course"].Value);
        d.school = Convert.ToString(selectedRow.Cells["school"].Value);
        d.level = Convert.ToString(selectedRow.Cells["level"].Value);
      }
      edCourse.Text = d.course;
      edSchool.Text = d.school;
      edLevel.Text = d.level;
      selectedDegree = d;
      button10.Text = "Save";
      sDegree.Text = "Deselect";
    }

    private void eAddresses_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      Address a = new Address();
      if (eAddresses.SelectedCells.Count > 0)
      {
        int selectedrowindex = eAddresses.SelectedCells[0].RowIndex;
        DataGridViewRow selectedRow = eAddresses.Rows[selectedrowindex];
        a.country = Convert.ToString(selectedRow.Cells["country"].Value);
        a.postalcode = Convert.ToString(selectedRow.Cells["postalcode"].Value);
        a.city = Convert.ToString(selectedRow.Cells["city"].Value);
        a.street = Convert.ToString(selectedRow.Cells["street"].Value);
        a.num = Convert.ToString(selectedRow.Cells["num"].Value);
      }
      edCountry.Text = a.country;
      edPostalcode.Text = a.postalcode;
      edCity.Text = a.city;
      edStreet.Text = a.street;
      edNumber.Text = a.num;
      selectedAddress = a;
      button9.Text = "Save";
      sAddress.Text = "Deselect";
    }

    private void ePositions_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      Position p = new Position();
      if (ePositions.SelectedCells.Count > 0)
      {
        int selectedrowindex = ePositions.SelectedCells[0].RowIndex;
        DataGridViewRow selectedRow = ePositions.Rows[selectedrowindex];
        p.posId = Convert.ToInt32(selectedRow.Cells["posId"].Value);
        p.posName = Convert.ToString(selectedRow.Cells["posName"].Value);
        p.description = Convert.ToString(selectedRow.Cells["description"].Value);
        p.hour_fee = Convert.ToDouble(selectedRow.Cells["hour_fee"].Value);
      }
      comboBox4.SelectedIndex = comboBox4.FindStringExact(Convert.ToString(p.posName));
      selectedPosition = p;
    }

    private void button5_Click(object sender, EventArgs e)
    {
      //Update employee
      int bsn = Convert.ToInt32(edBSN.Text);
      string name = edName.Text;
      string surname = edSurname.Text;
      int employeeId = selectedEmployee_.employeeId;

      DBConnection newC = new DBConnection();
      newC.Execute("UPDATE employees SET bsn = '" + bsn + "', name = '" + name + "', surname = '" + surname + "' "
                  +"WHERE employeeId = '" + employeeId + "'");
      reloadEmployees();
    }

    private void button10_Click(object sender, EventArgs e)
    {
      // Add/update degree
      string course = edCourse.Text;
      string school = edSchool.Text;
      string level = edLevel.Text;
      int employeeId = selectedEmployee_.employeeId;
      int degreeId = selectedDegree.degreeId;
      DBConnection newC = new DBConnection();
      if(selectedDegree.degreeId > 0)
      {
        newC.Execute("UPDATE degrees SET course = '" + course + "', school = '" + school + "', level = '" + level + "' "+
                     "WHERE degreeId = '" + degreeId + "'");
      }
      else
      {
        newC.Execute("INSERT INTO degrees (employeeId, course, school, level) "+
                     "VALUES ('" + employeeId + "', '" + course + "', '" + school + "', '" + level + "')");
      }
    }

    private void button9_Click(object sender, EventArgs e)
    {
      // Add/update address
      string oldCountry = selectedAddress.country;
      string oldPostalcode = selectedAddress.postalcode;
      string country = edCountry.Text;
      string postalcode = edPostalcode.Text;
      string city = edCity.Text;
      string street = edStreet.Text;
      string num = edNumber.Text;
      int employeeId = selectedEmployee_.employeeId;
      DBConnection newC = new DBConnection();
      if(selectedAddress.country != null && selectedAddress.postalcode != null)
      {
        newC.Execute("UPDATE addresses SET country = '" + country + "', postalcode = '" + postalcode + "', "+
                     "city = '" + city + "', street = '" + street + "', num = '" + num + "'"+
                     "WHERE country = '" + oldCountry + "' AND postalcode = '" + oldPostalcode + "'");
      }
      else
      {
        newC.Execute("INSERT INTO addresses (country, postalcode, city, street, num)"+
                     "VALUES ('" + country + "', '" + postalcode + "', '" + city + "', '" + street + "', '" + num + "')");
        int adrId = newC.ExecutePK("INSERT INTO eaddresses (employeeId, country, postalcode)"+
                     "VALUES ('" + employeeId + "', '" + country + "', '" + postalcode + "')");
        newC.Execute("UPDATE employees SET employeeAid = '" + adrId + "' WHERE employeeId = '" + employeeId +"'");
      }
    }

    private void button8_Click(object sender, EventArgs e)
    {
      //Delete employee
      DBConnection newC = new DBConnection();
      int employeeId = selectedEmployee_.employeeId;
      var addresses = newC.Select("SELECT * FROM eaddresses WHERE employeeId = '" + employeeId + "'");
      List<Address> delAddress = new List<Address>();
      foreach (List<string> entry in addresses)
      {
        Address d = new Address();
        d.country = entry[2];
        d.postalcode = entry[3];
        delAddress.Add(d);
      }
      newC.Execute("DELETE FROM employees WHERE employeeId = '" + employeeId + "'");
      foreach(Address del in delAddress)
      {
        newC.Execute("DELETE FROM addresses WHERE country = '" + del.country + "' AND postalcode = '" + del.postalcode + "'");
      }
      reloadEmployees();
    }

    private void button6_Click(object sender, EventArgs e)
    {
      //Delete degree from employee
      Degree d = selectedDegree;
      DBConnection newC = new DBConnection();
      newC.Execute("DELETE FROM degrees WHERE degreeId = '" + d.degreeId + "'");
      fillDegrees(selectedEmployee_.employeeId, eDegrees);
    }

    private void button7_Click(object sender, EventArgs e)
    {
      //Delete address from employee
      Address a = selectedAddress;
      //DBConnection newC = new DBConnection();
      //newC.Execute("DELETE FROM addresses WHERE country = '" + a.country + "' AND postalcode = '" + a.postalcode + "'");
    }

    public void fillDegrees(int employeeId, DataGridView dGrid)
    {
      DBConnection newC = new DBConnection();
      var degrees = newC.Select("SELECT * FROM degrees WHERE employeeId = '" + employeeId + "'");
      List<Degree> dList = new List<Degree>();
      foreach (List<string> entry in degrees)
      {
        Degree d = new Degree();
        d.degreeId = Convert.ToInt32(entry[0]);
        // d.employeeid = Convert.ToInt32(entry[1]);
        d.course = entry[2];
        d.school = entry[3];
        d.level = entry[4];
        dList.Add(d);
      }
      BindingList<Degree> eBindingList = new BindingList<Degree>(dList);
      BindingSource eSource = new BindingSource(eBindingList, null);
      dGrid.DataSource = eSource;
    }

    public void fillAddresses(int employeeId, DataGridView aGrid)
    {
      DBConnection newC = new DBConnection();
      var addresses = newC.Select("SELECT * FROM eaddresses INNER JOIN addresses " +
                                  "ON eaddresses.country = addresses.country and eaddresses.postalcode = addresses.postalcode " +
                                  "WHERE eaddresses.employeeId = '" + employeeId + "'");
      List<Address> aList = new List<Address>();
      foreach (List<string> entry in addresses)
      {
        Address a = new Address();
        a.country = entry[2];
        a.postalcode = entry[3];
        a.city = entry[6];
        a.street = entry[7];
        a.num = entry[8];
        aList.Add(a);
      }
      BindingList<Address> aBindingList = new BindingList<Address>(aList);
      BindingSource aSource = new BindingSource(aBindingList, null);
      aGrid.DataSource = aSource;
    }

    public void fillPositions(int employeeId, DataGridView pGrid)
    {
      DBConnection newC = new DBConnection();
      var positions = newC.Select("SELECT * FROM epositions " +
                            "INNER JOIN positions ON epositions.posId = positions.posId " +
                            "WHERE employeeId = '" + employeeId + "'");
      List<Position> pList = new List<Position>();
      foreach (List<string> entry in positions)
      {
        Position p = new Position();
        p.posId = Convert.ToInt32(entry[0]);
        p.posName = entry[5];
        p.description = entry[6];
        p.hour_fee = Convert.ToDouble(entry[7]);
        if(entry[3] != "")
        {
          p.projectId = Convert.ToInt16(entry[3]);
        }
        pList.Add(p);
      }
      BindingList<Position> pBindingList = new BindingList<Position>(pList);
      BindingSource pSource = new BindingSource(pBindingList, null);
      pGrid.DataSource = pSource;
    }

    public void fillComboBoxHeadquarters(ComboBox myCombo)
    {
      DBConnection newC = new DBConnection();
      var headquarters = newC.Select("SELECT * FROM headquarters");
      List<Item> items = new List<Item>();
      foreach (List<string> entry in headquarters)
      {
        items.Add(new Item() { Text = entry[0], Value = entry[0] });
      }
      myCombo.DataSource = items;
      myCombo.DisplayMember = "Text";
      myCombo.ValueMember = "Value";
    }

    public void fillComboBoxPositions(ComboBox myCombo)
    {
      DBConnection newC = new DBConnection();
      var positions = newC.Select("SELECT * FROM positions");
      List<Item> posItems = new List<Item>();
      foreach (List<string> entry in positions)
      {
        posItems.Add(new Item() { Text = entry[1], Value = entry[0] });
      }
      myCombo.DataSource = posItems;
      myCombo.DisplayMember = "Text";
      myCombo.ValueMember = "Value";
    }

    public void fillComboBoxProjects(ComboBox myCombo)
    {
      DBConnection newC = new DBConnection();
      var projects = newC.Select("SELECT * FROM projects");
      List<Item> projItems = new List<Item>();
      foreach (List<string> entry in projects)
      {
        projItems.Add(new Item() { Text = entry[0], Value = entry[0] });
      }
      myCombo.DataSource = projItems;
      myCombo.DisplayMember = "Text";
      myCombo.ValueMember = "Value";
    }

    private void sDegree_Click(object sender, EventArgs e)
    {
      edCourse.Text = "";
      edSchool.Text = "";
      edLevel.Text = "";
      sDegree.Text = "No selection";
      selectedDegree = new Degree();
    }

    private void sAddress_Click(object sender, EventArgs e)
    {
      edCountry.Text = "";
      edPostalcode.Text = "";
      edCity.Text = "";
      edStreet.Text = "";
      edNumber.Text = "";
      sAddress.Text = "No selection";
      selectedAddress = new Address();
    }

    private void label41_Click(object sender, EventArgs e)
    {
      //Selected
      selectedPosition = new Position();
      sPosition.Text = "No selection";
    }

    private void sProject_Click(object sender, EventArgs e)
    {
      sProject.Text = "No selection";
      ePbudget.Text = "";
      ePhours.Text = "";
      selectedProject = new Project();
    }

    private void button12_Click(object sender, EventArgs e)
    {
      // Remove position 
      Position p = selectedPosition;
      if(p.posId > 0)
      {
        DBConnection newC = new DBConnection();
        newC.Execute("DELETE FROM epositions WHERE ePosId = '" + p.posId + "'");
      }
      else
      {
        ErrorL.Text = "No position selected";
      }
    }

    private void button11_Click(object sender, EventArgs e)
    {
      // Add position
      int posId = Convert.ToInt32(comboBox4.SelectedValue);
      int employeeId = selectedEmployee_.employeeId;
      int projId = Convert.ToInt32(comboBox5.SelectedValue);
      DBConnection newC = new DBConnection();
      newC.Execute("INSERT INTO epositions (posId, employeeId, projectId)"+
                   "VALUES ('" + posId + "', '" + employeeId + "', '" + projId + "')");

    }

    private void button13_Click(object sender, EventArgs e)
    {
      // Add project
      string hqName = Convert.ToString(comboBox6.SelectedValue);
      string budget = pBudget.Text;
      string hours = pHours.Text;

      DBConnection newC = new DBConnection();
      newC.Execute("INSERT INTO projects (hqName, budget, hours)"+
                   "VALUES ('" + hqName + "', '" + budget + "', '" + hours + "')");
      reloadProjects();
    }

    private void button14_Click(object sender, EventArgs e)
    {
      // Edit project
      int projectId = selectedProject.projectId;
      string hqName = Convert.ToString(comboBox7.SelectedValue);
      string budget = ePbudget.Text;
      string hours = ePhours.Text;
      DBConnection newC = new DBConnection();
      newC.Execute("UPDATE projects SET hqName = '" + hqName + "', '" + budget + "', '" + hours + "' WHERE projectId = '" + projectId + "'");
      reloadProjects();
    }

    private void button15_Click(object sender, EventArgs e)
    {
      // Delete project
      int projectId = selectedProject.projectId;
      DBConnection newC = new DBConnection();
      newC.Execute("DELETE FROM projects WHERE projectId = '" + projectId + "'");
      reloadProjects();
      selectedProject = new Project();
    }
  }
}
