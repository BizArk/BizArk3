# StringTemplate
StringTemplate Provides a way to use property names instead of indexes in text to format a string. So instead of "Hello {0}" you can have "Hello {name}". 

Internally, StringTemplate simply parses the string and converts it into a standard formatted string with indexed place holders, but it keeps track of the order the properties are in and, when you format the template, puts the properties in the correct order for you. This means that you still get all the same formatting options as regular formatted strings but with the convenience of named place holders.

## Examples
1. Simple formatted string.
```C#
var template = new StringTemplate("Hello {name}");
var str = template.Format(new { name = "Bob" });
```

2. Supports standard formatting.
```C#
var template = new StringTemplate("Amount due: {amount:C}\nDue by: {dueDate:d}");
var str = template.Format(new { amount = 12.34m, dueDate = DateTime.Parse("3/30/2016") });
```

3. Too verbose? Use a shortcut!
```C#
using BizArk.Core.Extensions.StringExt;
var str = "Hello {name}".Tmpl(new { name = "Bob" });
```

4. Have nested objects? Lists? Dictionary? No problem. Uses ObjectDictionary to support complex syntax.
```C#
var obj = GetMySuperComplexObject();
var str = "Hello {Patient.Name}. You have an appointment on {Appointments[0].Date:MMM dd, yyyy @ hh:mm tt}".Tmpl(obj);
```

5. StringTemplate also supports anything that can be converted to a PropertyBag, including DataRow, IDataReader, etc.