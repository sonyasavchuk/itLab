using Xunit;
using itLab1;

public class dbManagerTests
{
    [Fact]
    public void CreateDB_ValidName_ReturnsTrue()
    {
        // Arrange
        dbManager dbm = new dbManager();

        // Act
        bool result = dbm.CreateDB("TestDB");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateDB_EmptyName_ReturnsFalse()
    {
        // Arrange
        dbManager dbm = new dbManager();

        // Act
        bool result = dbm.CreateDB("");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AddTable_ValidName_ReturnsTrue()
    {
        // Arrange
        dbManager dbm = new dbManager();
        dbm.CreateDB("TestDB");

        // Act
        bool result = dbm.AddTable("Table1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AddTable_EmptyName_ReturnsFalse()
    {
        // Arrange
        dbManager dbm = new dbManager();
        dbm.CreateDB("TestDB");

        // Act
        bool result = dbm.AddTable("");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AddColumn_ValidInput_ReturnsTrue()
    {
        // Arrange
        dbManager dbm = new dbManager();
        dbm.CreateDB("TestDB");
        dbm.AddTable("Table1");

        // Act
        bool result = dbm.AddColumn(0, "Column1", "String");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AddColumn_InvalidTable_ReturnsFalse()
    {
        // Arrange
        dbManager dbm = new dbManager();
        dbm.CreateDB("TestDB");

        // Act
        bool result = dbm.AddColumn(0, "Column1", "String");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AddRow_ValidInput_ReturnsTrue()
    {
        // Arrange
        dbManager dbm = new dbManager();
        dbm.CreateDB("TestDB");
        dbm.AddTable("Table1");
        dbm.AddColumn(0, "Column1", "String");

        // Act
        bool result = dbm.AddRow(0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ChangeValue_ValidInput_ReturnsTrue()
    {
        // Arrange
        dbManager dbm = new dbManager();
        dbm.CreateDB("TestDB");
        dbm.AddTable("Table1");
        dbm.AddColumn(0, "Column1", "String");
        dbm.AddRow(0);

        // Act
        bool result = dbm.ChangeValue("NewValue", 0, 0, 0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Validation_IntegerType_ValidValue_ReturnsTrue()
    {
        // Arrange
        myType type = new myType("Integer");

        // Act
        bool result = type.Validation("123");

        // Assert
        Assert.True(result);
    }


    [Fact]
    public void Validation_RealType_ValidValue_ReturnsTrue()
    {
        // Arrange
        myType type = new myType("Real");

        // Act
        bool result = type.Validation("12.34");

        // Assert
        Assert.True(result);
    }


    [Fact]
    public void Validation_CharType_ValidValue_ReturnsTrue()
    {
        // Arrange
        myType type = new myType("Char");

        // Act
        bool result = type.Validation("A");

        // Assert
        Assert.True(result);
    }



    [Fact]
    public void Validation_StringType_ValidValue_ReturnsTrue()
    {
        // Arrange
        myType type = new myType("string");

        // Act
        bool result = type.Validation("abc");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Validation_DateType_ValidValue_ReturnsTrue()
    {
        // Arrange
        myType type = new myType("date");

        // Act
        bool result = type.Validation("2023-10-12");

        // Assert
        Assert.True(result);
    }

}
