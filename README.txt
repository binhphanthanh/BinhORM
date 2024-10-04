Preface: ...


Table of content

1. Feature briefing
2. Getting started - create your first application
	a. Download
	b. Configuration
	c. Create entity class
	d. Cerate a Data Access class
	e. Create a service class (Business layer)
	f. Create your application presentation
3. Advance data access layer
	a. Basic functions
	b. Get data in to custom object, not an entity.
	c. Fill data by object's contructor in sql
4. Advance service layer (Business layer)
5. Data binding
6. Extend the framework to support more databases
7. Framework architecture
8. History of the framework

1. Feature briefing
- Half O/R mapping framework. (using native sql instead of the framework sql)
- Support loading any type of data transfer object without any mapping or configuration, just only follow a simple naming convention.
- Support easy transaction management. 
- Multi-layer framework (Entity - Data access layer, Service layer (business logic layer))
- Suport multi-lingual for data.
- Support 2 ways data binding entity in WinForm.
- Suport a powerful object collection and a collection view (like data view) with search, sort, filter... It's very useful
 in WinForm.
- Support multiple database (SqlServer, MySql, OleDB) and can be extends to another database easily.
- If you use standard sql only, you can run your application on many database without changing the code.
- Easy to use, all your data access code now only focus on how to write correct SQLs. No more duplication code in the data access layer.

1. Getting started - create your first application
a. Download
You can download the source code, DLL, and demo application and database here.
http://sites.google.com/site/binhphanclub/course-materials/BinhORM.zip?attredirects=0

This is the Entity Creator
http://sites.google.com/site/binhphanclub/course-materials/CreatePureObjectWizard.zip?attredirects=0

b. Configuration
- Create a project, web or winform are fine. 
- Add reference for BinhORM.dll.
- If you use MySQL, you add reference for the MySQL driver MySQL.Data.dll
- Open web.config (ASP.NET) or app.config (Winform) and add the follow connection configuration:

* This is the configuration for MySql
<configuration>
  <appSettings>
    <add key="factory" value="SystemFramework.Common.QueryBase.MySql.MySqlFactory"/>
  </appSettings>
  <connectionStrings>
    <add name="connection" connectionString="Server=localhost;Uid=root;Pwd=123456;Database=testframework;charset=utf8;"/>
  </connectionStrings>
<configuration>

* Sql Server (replace the above config with these lines)
<add name="connection" connectionString="Data Source=.\sqlexpress;Initial Catalog=testframework;User Id=sa;Password=123456"/>
<add key="factory" value="SystemFramework.Common.QueryBase.Sql.SqlServerFactory"/>

* Any database use OleDb (this is for Ms Access)
<add name="connection" connectionString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\TestFrameWork.mdb;User Id=admin;Password=;" />
<add key="factory" value="SystemFramework.Common.QueryBase.OleDb.OleDbFactory" />

c. Create entity class

[Serializable]
[Table(Name = "Customer")]
public class Customer : BaseEntity
{
	private int _id;
	private string _name;           

	public Customer ()
	{
	}

	[Key(AutoIncrement = true)]
	[Column(MappedName = "ID")]
	public int Id
	{
		get { return this._id; }
		set { _id = value }
	}
	
	[Column(MappedName = "name")]
	public string Name
	{
		get { return this._name; }
		set { _name = value }
	}
}

You can use auto properties in C# 3.5 like this:

[Serializable]
[Table(Name = "Customer")]
public class Customer : BaseEntity
{      
	public Customer ()
	{
	}

	[Key(AutoIncrement = true)]
	[Column(MappedName = "ID")]
	public int Id {	get; set; }
	
	[Column(MappedName = "name")]
	public string Name { get; set; }
}

Attributes explaination:

- The entity must extends the BaseEntity class
- [Serializable] : this is an optional in case you want to serialize it (such as webservice).
- [Table(Name = "Customer")] : This attribute map this object to the table Customer in the database.
- [Column(MappedName = "name")] : This attribute map this property to column name in table Customer.
- [Key(AutoIncrement = true)] : This attribute indicates that this property is a key column of the table. This column is auto-increament in the database. The default value is false. So, if your ID is not auto-increase, you only need to declare [Key()]
- If your table has a compound key, you just need to put [Key()] attribute to those properties like this:

[Key()]
[Column(MappedName = "Column1")]
public int Column1{ get; set; }

[Key()]
[Column(MappedName = "Column2")]
public int Column2{ get; set; }

d. Cerate a Data Access class

public class CustomerDAO : BaseDataAccess
{
	public CustomerDAO(BaseBusiness baseBusiness)
		: base(baseBusiness)
	{
	}

	public IList<Customer> GetCustomers()
	{
		string sql = "SELECT * FROM Customer where id > @id";
		return GetList<Customer>(sql, "id", 3);
	}
}

Explaination:

- Your data access class must extend BaseDataAccess - a common class of the framework.
- Your data access class must have the constructor with BaseBusiness parameter like above.
- GetList is one of the function of the BaseDataAccess class.
- The parameters passed to the GetList by pair. The param name go fist then the value. You can pass as many parameters as you want. For example if your sql have 2 parameters, you pass them to the method like this: GetList<Customer>(sql, "id", 3, "type", "customer");

e. Create a service class (Business layer)

public class CustomerBO : BaseBusiness
{
	private CustomerDAO khDAO;

	public CustomerBO()
	{
		khDAO = new CustomerDAO(this);
	}

	public IList<Customer> GetCustomers()
	{
		return khDAO.GetCustomers();
	}
}

Explaination:

- Your business class must extend BaseBusiness - a common class of the framework
- All data access class created in this service class must pass this service in to the constructor: khDAO = new CustomerDAO(this);

f. Create your application presentation

You can call the service class any where in your application like this:
IList<Customer> customers = new CustomerBO().GetCustomers();

2. Advance data access layer

Big notice: This framework use native query of a specific DBMS. It does not have it own query language like JPA or Hibernate.

a. Basic functions
The framework provides some common method for handling data:

//Insert a record to table Customer
public int InsertCustomer(Customer entity)
{
	return Insert(entity);
}

//Update a record to table Customer
public int UpdateCustomer(Customer entity)
{
	return Update(entity);
}

//Delete a record from table Customer
public int DeleteCustomer(long id)
{
	return base.Delete<Customer>(id);
}

//Get a customer by its id
public Customer GetCustomer(long id)
{
	return Find<Customer>(id);
}

//Get a list of records
public IList<Customer> GetCustomers()
{
	string sql = "SELECT * FROM Customer where id > @id";
	return GetList<Customer>(sql, "id", 3);
}

This is the basic functions API
- Insert an entity
    int Insert<T>(T entity) where T : BaseEntity;
- Update an entity
    int Update<T>(T entity) where T : BaseEntity;
- Delete an entity in case your entiy has a compound key. To create a KeyValue, you can do this: Customer.CreateKey<Customer>(value1, value2); the parameters in the order you place the key properties.
    int Delete<T>(KeyValue id) where T : BaseEntity;
- Delete an entity has simple key or compoind key, just pass the parameters in the order you place the key properties.
    int Delete<T>(params object[] ids) where T : BaseEntity;

- Insert/Delete/Update a list of entity depend on the state of each entity. This function is usefull for Windows application with data binding. In web application, it is useless
	int BatchUpdate<T>(IList<T> objectList) where T : BaseEntity;

- Execute a SQL statement.
    int ExecuteNonQuery(DbCommand sqlCommand);
    int ExecuteNonQuery(string sql, object entity);
    int ExecuteNonQuery(string sql, params object[] args);

- Get a DbDataReader and get data by yourself
    DbDataReader ExecuteReader(DbCommand sqlCommand);
- After complete getting data, you must close the reader by calling this function.
    void CompleteReader(DbDataReader reader);

- Get the first column of the first record of the result. It is useful for count sql or something like this.
    object ExecuteScalar(string sql, object entity);
    object ExecuteScalar(DbCommand sqlCommand);
    object ExecuteScalar(string sqlString, params object[] args);

- Get an entity by its Id
    T Find<T>(params object[] ids) where T : BaseEntity;

- Get a single data objects
    T LoadData<T>(string selectSql, params object[] args);
    T LoadData<T>(DbCommand selectCommand);

- Get a list of data objects
    IList<T> GetList<T>(string selectSql, int startRecord, int maxResult, params object[] args);
    IList<T> GetList<T>(string selectSql, params object[] args);
    IList<T> GetList<T>(DbCommand selectCommand);
    IList<T> GetList<T>(DbCommand selectCommand, int startRecord, int maxResult);

b. Get data in to custom object, not an entity.
What will you do if you want to load one or a list of custom objects that is not an entity? This is the steps:
- Create you custom object with properties you like such as:
	
public class Temp
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string WhatEver { get; set; }
}

- Then call the framework functions as normal:
//Load an object, it will return null if there is no result.
string sql = "select id, name, 'some thing' as WhatEver from Customer where id = @id";
Temp t = LoadData<Temp>(sql, "id", 3);

//Load a list objects, it will return an empty list if there is no result.
IList<Temp> result = GetList<Temp>(sql, "id", 3);

Note that there is only one convention is that the property name must match with the name of the select column names. It is case-insensitive.

c. Fill data by object's contructor in sql
- If the properties of your onject class do not match with the selected column names nad you dont want to change it? No proplem, you can use the contructor of your onject like this:
- Create a constructor for your object for example:

public Temp(long id, string name, string whatEver)
{
	_id = id;
	_name = name;
	_whatEver = whatEver;
}

- Write the sql like this then execute it:
string sql = "select new BusinessEntity.Temp(id, name, 'some thing') from Customer where id = @id";
Temp t = LoadData<Temp>(sql, "id", 3);

or
	
IList<Temp> result = GetList<Temp>(sql, "id", 3);

Using the constructor in the sql must be full qualified namespace. The order of the list paramerters in sql must match with the order of parameters in the constructor. This look like JPA and hibernate.

3. Advance service layer (Business layer)

A persistence framework cannot lack of transaction management. This framework also have this feature, for example:

public class CustomerBO : BaseBusiness
{
	private CustomerDAO khDAO;

	public CustomerBO()
	{
		khDAO = new CustomerDAO(this);
	}
	
	public void ActionWithTransaction()
	{
		try
		{
			BeginTransaction();
			// Do many actions like insert/delete/update here
			CommitTransaction();
		}
		catch (Exception)
		{
			RollBackTransaction();
		}
	}
}

If you want to put diference data access classes in the same transaction, you can do this:
public void ActionWithManyDAO()
{
	try
	{
		CustomerDAO khDAO = new CustomerDAO(this);
		RepoDAO repoDAO = new RepoDAO(this);

		BeginTransaction();
		khDAO.DoSomeThing();
		repoDAO.DoSomeThingElse();		
		CommitTransaction();
	}
	catch (Exception)
	{
		RollBackTransaction();
	}            
}

Some time you want to reuse a function in another service class in a service class and you want it to have the transaction, you can do this:

public void ActionWithOtherBO()
{
	try
	{
		RepoBO repoBO = new RepoBO();
		Join(repoBO); //Join this BO to the current transaction

		BeginTransaction();
		khDAO.DoSomeThing();
		repoBO.DoSomeThingElse();			
		CommitTransaction();

		//If you want to detach the repoBO out of the transaction
		UnJoin(repoBO);
	}
	catch (Exception)
	{
		RollBackTransaction();
	}
}

4. Data binding
(For this part, I used an opensource called BindingListView and I wrapped it again)
	
In window form applications, 2 ways data binding is a great feature that Microsoft provide us. When you edit the content of a UI component, its data is automatically update to your data object. And vice versa, when you set a value to your data object, it will be display imediatly in the UI without any UI handle. This feature can be accomplete with DataTable, DataRow od ADO.NET. And I make it available for our data objects.

You should change your set method in the data object like this:

public string Name
{
	get { return this._name; }
	set { UpdateValue(ref _name, value, "Name"); }
}

- UpdateValue is a function of BaseEntity class.
- first parameter of the function UpdateValue is the class's variable 
- The third parameter id the name of this property - it is case sensitive.
- Then, you can bind this object to any UI control you like:

Customer c = new CustomerBO().GetCustomer(3);
txtName.DataBindings.Add("Text", c, "Name", false, DataSourceUpdateMode.OnPropertyChanged);

To bind a list to DataGridView, you can use ObjectCollection và ObjectCollectionView class.
- ObjectCollection: The same as DataTable
- ObjectCollectionView: The same as DataView
- Each item in ObjectCollectionView is an ObjectView. ObjectView is the same as DataRowView
- Each item in ObjectCollection is your data object.

The ObjectCollectionView is more advantage than the ObjectCollection is that it can filter the collection without loosing any data in the collection. You dont need to query the database again and again for the sub set of data.

How to use it?

CustomerBO bo = new CustomerBO();
ObjectCollection<Customer> list = new ObjectCollection<Customer>(bo.GetCustomers());
ObjectCollectionView<Customer> listView = new ObjectCollectionView<Customer>(list);
dataGridView1.DataSource = listView;

- When you filter the ObjectCollectionView, the DataGridView will automatically update it data without any update UI action.
- For example, you type in a textbox and you want the DataGridView seach all Customer name like the textbox value, you can do this:
	
private void textBox1_TextChanged(object sender, EventArgs e)
{
	listView.ApplyFilter(
	   delegate(Customer item)
	   {
		   return string.IsNullOrEmpty(textBox1.Text)
					|| item.Name.ToLower().Contains(textBox1.Text.ToLower());
	   }
   );
}

6. Multi-lingual
- To support multilingual for some of your data, you need follow 2 naming convention with 2 tables.
	. The main table, you can name it what ever you like. This table only contains locale independent data like Date Of Birth, quantity of something, color...
	. The language table name must the same with the main table and have the postfix _Lang.
	. The primary key in the language table is including the primary key of the main table with a column LanguaKey (string). The name of the primary key must have the prefix L. Please see the below picture for more infomation.

(picture)

How do the framework support multi-lingual?
a. Create an entity for multi-lingual data
- Declare a property MultiLanguage = true in the Table attribute to support multi-ligual for this table. For exmaple:
[Table(Name = "Customer", MultiLanguage = true)]
- For the columns that need to support multi-ligual, also add the property MultiLanguage = true. For example:
[Column(MappedName = "name", MultiLanguage = true)]
	
The below is an example multi-ligual entity

[Serializable]
[Table(Name = "Customer", MultiLanguage = true)]
public class Customer : BaseEntity
{
	[Key(AutoIncrement = true)]
	[Column(MappedName = "id")]
	public long Id { get; set; }

	[Column(MappedName = "name", MultiLanguage = true)]
	public string Name { get; set; }

	[Column(MappedName = "ADDRESS", MultiLanguage = true)]
	public string Address { get; set; }
	}

	[Column(MappedName = "PHONE")]
	public string Phone { set; set; }
}

b. Use it

* You don't care about the 2 tables when insert/update/delete data. All thing you do are supply the data and a language, then call an appropriate funtion. For example:

- Insert a record with English
Customer c = new Customer();
c.Name = "Phan Thanh Binh"
...
c.LanguageKey = "en-US";
base.Insert<Customer>(c);

- Update it with another language
Customer c2 = new Customer();
c2.Id = c1.Id;
c.Name = "Phan Thanh Bình"
...
c.LanguageKey = "vi-VN";
base.Update<Customer>(c2);

- Delete a record with Id = 253:
base.Delete<Customer>(253);

* Support simple query.
- Find a record by Id and a language
base.FindByLanguage<Customer>("en-US", 253);
- Find a record by a complex condition:
base.LoadDataByLanguage<Customer>("name like '%binh%'", "en-US"); or with parameter
base.LoadDataByLanguage<Customer>("name like '%' + @name + '%'", "en-US", "name", "binh");
- Find a list of data by a comlex condition:
base.GetListByLanguage<Customer>("name like '%' + @name + '%'", "en-US", "name", "binh");
with pagination: from record 10 and maximum 20 records.
base.GetListByLanguage<Customer>("name like '%' + @name + '%'", "en-US", 10, 20, "name", "binh");

* For a complex query that need to join many tables, ypu must write your own sql. for example:

string sql = @"select * 
			from Customer c inner join Customer_Lang l on c.id = l.lid 
			where LanguageKey = @language";
base.GetList<Customer>(sql, "language", "vi-VN");

7. Testing

- The zip package contains UnitTestBinhORM that perform some unit tests for the framework. Unit test is performed properly in SQl Server. I have not enough time to prepare for MySQL and MS Access.

- It also have the performance test for loading data named: PerformanceTestBinhORM. The below is the result of performance testing.

Environment: Sql Server Express 2005, CPU Core 2 dual 1.86Ghz, 2GB DDR2 533
Compare BinhORM with ADO.NET using SqlDataAdapter and DataSet.

- In the first result, loading over 8000 records using nornal GetData with the sql
@"select * 
from Customer c inner join Customer_Lang l on c.id = l.lid 
where LanguageKey = 'vi-VN'"
The SqlDataAdapter absolutely win the performnace.

- In the second result, I using constructor for loading data for BinhORM like this :
@"select new Common.Customer(id, name, Address, Phone, money, deleted) 
from Customer c inner join Customer_Lang l on c.id = l.lid 
where LanguageKey = 'vi-VN' "
With over 8000 records, the SqlDataAdapter slightly faster then BinhORM.

- The third result, I only load 50 record with pagination with this function.
base.GetList<Customer>(@"select *
						from Customer c inner join Customer_Lang l on c.id = l.lid 
						where LanguageKey = 'vi-VN' ", 0, 50);
					
And the SqlDataAdapter with the same sql:
SqlDataAdapter da = new SqlDataAdapter(@"select * from Customer c inner join Customer_Lang l on c.id = l.lid 
                                         where LanguageKey = 'vi-VN'", conn);
DataSet ds = new DataSet();
da.Fill(ds, 0, 50, "customer");

The result is impressive, my framework absolutly win the performance about 5.5 time.

- The fourth result is more impressive when I use loading by constructor with the sql:
base.GetList<Customer>(@"select new Common.Customer(id, name, Address, Phone, money, deleted) 
						from Customer c inner join Customer_Lang l on c.id = l.lid 
						where LanguageKey = 'vi-VN' ", 0, 50);

It is about 22.5 times fater than the SqlDataAdapter.

Conclusion:
	- In an application, how many functions that you want to load thoundsands of records a time? I think it is very rare and almost you want about less tham 100 records in a query.
	- So the the framework absolutely acceptable about the performance.
	- To speed up the loading time, you should load data using constructor. Because when we use a normal mapping, it will use the property to set data. An the reflection is not good at the performance compare to call the property directly. When we use contructor, it will reduce the number of reflection call dramatically.

8. Framwwork structure
	
8. Extend the framework to support more databases
- You should extends 3 classes: ParameterManager, DataManager and IDbFactory for a specific DBMS.
- Change the config in your application to:

<add key="factory" value="YourNameSpace.YourFactoryClassName, YourDLLName"/>
	
You can refer to the source code of the framework to make the appropriate extention.

