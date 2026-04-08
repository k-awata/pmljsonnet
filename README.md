# PMLJsonNet

PMLJsonNet provides an interface for processing JSON in AVEVA PML2.

## Installation

1. Download the ZIP archive from [Releases](https://github.com/k-awata/pmljsonnet/releases).

2. Extract the files and folders from the ZIP archive and place them in a directory specified by the `PMLLIB` environment variable.

3. Launch an AVEVA product.

4. Enter the following command in the Command Window:

   ```pml2
   pml rehash all
   ```

## Usage

### `.Parse(!json is STRING, !object is ANY)`

The Parse method parses a JSON string and stores the result in the specified object.

- Example:

  ```pml2
  using namespace 'PMLJsonNet'
  !json = object PMLJSON()
  !obj = object POSITION()
  !json.Parse('{"east":123,"north":456,"up":789,"origin":"/*"}', !obj)
  q var !obj
  ```

- Output in the Command Window:

  ```text
  <POSITION> E 123mm N 456mm U 789mm WRT /*
     EAST <REAL> 123
     NORTH <REAL> 456
     ORIGIN <DBREF> =****/0 (Refno depends on DB No.)
     UP <REAL> 789
  ```

### `.Parse(!file is FILE, !object is ANY)`

The Parse method parses a JSON file and stores the result in the specified object.

- Example:

  ```pml2
  using namespace 'PMLJsonNet'
  !json = object PMLJSON()
  !file = object FILE('%temp%\sample.json')
  !obj = object DICTIONARY()
  !json.Parse(!file, !obj)
  q var !obj.keys !obj.values
  q var !obj.values[1].keys !obj.values[1].values
  q var !obj.values[2]
  ```

- Contents of `sample.json`:

  ```text
  {
    "dictionary": { "foo": 123, "bar": 456, "baz": 789 },
    "array": ["foo", 123, true],
    "boolean": false,
    "real": 1.23e4,
    "string": "foo\\bar \"baz\"",
    "null": null
  }
  ```

- Output in the Command Window:

  ```text
  <ARRAY>
     [1]  <STRING> 'dictionary'
     [2]  <STRING> 'array'
     [3]  <STRING> 'boolean'
     [4]  <STRING> 'real'
     [5]  <STRING> 'string'
     [6]  <STRING> 'null'
  <ARRAY>
     [1]  <DICTIONARY> DICTIONARY
     [2]  <ARRAY> 3 Elements
     [3]  <BOOLEAN> FALSE
     [4]  <REAL> 12300
     [5]  <STRING> 'foo\bar "baz"'
     [6]  <STRING> Unset

  <ARRAY>
     [1]  <STRING> 'foo'
     [2]  <STRING> 'bar'
     [3]  <STRING> 'baz'
  <ARRAY>
     [1]  <REAL> 123
     [2]  <REAL> 456
     [3]  <REAL> 789

  <ARRAY>
     [1]  <STRING> 'foo'
     [2]  <REAL> 123
     [3]  <BOOLEAN> TRUE
  ```

### `.Stringify(!object is ANY) is STRING`

The Stringify method returns a JSON string generated from the specified object's data.

- Example:

  ```pml2
  using namespace 'PMLJsonNet'
  !json = object PMLJSON()
  !result = !json.Stringify( N WRT /* )
  $P$!result
  ```

- Output in the Command Window:

  ```text
  {"EAST":0,"NORTH":1,"ORIGIN":"/*","UP":0}
  ```

### `.WriteFile(!file is FILE, !object is ANY)`

The WriteFile method writes the specified object's data to a JSON file.

- Example:

  ```pml2
  using namespace 'PMLJsonNet'
  !json = object PMLJSON()
  !file = object FILE('%temp%\sample.json')
  !obj = object POINTVECTOR( E 123 N 456 U 789 WRT /* , U WRT /* )
  !json.WriteFile(!file, !obj)
  ```

- Output in `sample.json`:

  ```text
  {
    "DIRECTION": {
      "EAST": 0,
      "NORTH": 0,
      "ORIGIN": "/*",
      "UP": 1
    },
    "POSITION": {
      "EAST": "123mm",
      "NORTH": "456mm",
      "ORIGIN": "/*",
      "UP": "789mm"
    }
  }
  ```

### `.WriteFile(!file is FILE, !object is ANY, !indent is REAL)`

The WriteFile method writes the specified object's data to a JSON file using the specified indentation width.

- Example:

  ```pml2
  using namespace 'PMLJsonNet'
  !json = object PMLJSON()
  !file = object FILE('%temp%\sample.json')
  !obj = object DICTIONARY()
  !obj.SetValue('Array', Split('foo bar baz'))
  !obj.SetValue('Real', 12.3kg)
  !obj.SetValue('String', 'foo\bar "baz"')
  !obj.SetValue('DBRef', Nulref, true)
  !json.WriteFile(!file, !obj, 0)
  ```

- Output in `sample.json`:

  ```text
  {"Array":["foo","bar","baz"],"Real":"12.3kg","String":"foo\\bar \"baz\"","DBRef":null}
  ```

## Tests

The test cases use [PML Unit](https://github.com/PoByBolek/PmlUnit) and were run on Everything3D 2.1.

## License

[MIT License](LICENSE)
