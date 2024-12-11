# IdentifierGenerator

| ![NuGet Version](https://img.shields.io/nuget/v/IdentifierGenerator) | ![NuGet Downloads](https://img.shields.io/nuget/dt/IdentifierGenerator) ![Build](https://img.shields.io/github/actions/workflow/status/abock/idgen/build.yml) ![GitHub License](https://img.shields.io/github/license/abock/idgen) |
|:--|:--|
| **Install** | `dotnet tool install -g IdentifierGenerator` |
| **Update** | `dotnet tool update -g IdentifierGenerator`|

IdentifierGenerator (`idgen`) is a .NET global tool that supports the bulk generation of various kinds of unique<sup>1</sup> identifiers:

* [Versions 3 and 5 GUID/UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier#Versions_3_and_5_(namespace_name-based)): reproducible hashed namespace + name IDs
  * _Note: use version 5 (SHA-1) over version 3 (MD5) unless compatibility is required._
* [Version 4 GUID/UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier#Version_4_(random)): secure random IDs
* [Hashid](https://hashids.org/): Stable, short, unique, non-sequential, **_insecure_** IDs from integer array input
* [Nano ID](https://zelark.github.io/nano-id-cc/): secure random IDs of configurable size and alphabet
* Xcode Storyboard/XIB IDs

It can also encode strings for [3x4 numeric keypad/phone entry][phonewords]. This feature was implemented for and tested on [Fidelity's](https://www.fidelity.com/) antiquated phone authentication system, wherein they ask for an account password to be entered on a phone keypad.

> [!NOTE]
>
> _<sup>1.</sup> "Uniqueness" is an inherent property of the chosen format. No ID formats are truly unique._

## Examples
Run `idgen --help` after installing for detailed options.

> [!TIP]
> Any argument that expects a number my be specified in decimal, binary (`0b1001`), or hex (`0xabcd` and `ab123h`) notation. Numbers may also contain digit separators (`_` and `,`) and arbitrary whitespace.

### Generate a single v4 random GUID
```bash
idgen
```
> <pre>
> 0b0d5b33-b5e9-45cb-8f14-9bdab594cc98
> </pre>

### Do the same, but upper-case it
```bash
idgen -upper
```
> <pre>
> 8E350BC7-FF37-4E96-A5F7-CD945C9BDC33
> </pre>

### Generate a true base-64 encoded v4 random GUID
```bash
idgen -f base64
```
> <pre>
> JWn2giJJhUePnVzrCAK8JQ==
> </pre>

### Generate a true short v4 random GUID (base-64 with minor transformations)
```bash
idgen -f short
```
> <pre>
> 9lsQ5-h1nEy9uS3DMbLoeg
> </pre>

### Generate a Hashid from a set of numbers
```bash
idgen hashid 3 100 45 11
```
> <pre>
> Q0i67ixxir
> </pre>

### Generate a Hashid from a set of numbers with a minimum ID length, salt, and alphabet
```bash
idgen hashid 3 100 45 11 -salt bojangles -a aBcDeFgHiJkLmNoPqRsTuV -s 32
```
> <pre>
> BPDNPBNqkgLJVRsVaFLBsVJRVDmaeaLm
> </pre>

### Generate Hashids of numbers 1 to 100 from stdin
```bash
seq 1 100 | idgen hashid
```
> <pre>
> jR
> k5
> l5
> ...
> vgm
> wjR
> g56
> </pre>

### Generate 5 Nano IDs
```bash
idgen nanoid -n 5
```
> <pre>
> Fm82ZL3eyabMeVAgDGF7k
> LlrnWI3YrhUbQY3zHiyYc
> JUExm8eTVmLjLBjVeabZd
> 1bNIDlndN6W~chHMDq2y9
> izaokjb4E9ft6~rAgINEy
> </pre>

### Generate a Nano ID with a custom size and alphabet
```bash
idgen nanoid -s 32 -alphabet abcdefghijklmnopqrstuvwxyz
```
> <pre>
> aqmtbhpgomnpvudpmtesoooakyrrdrap
> </pre>

### Generate a v5 SHA-1 namespace + name hashed GUID using the URL namespace and the name _`bojangles`_
```bash
idgen v5 bojangles
```
> <pre>
> 11de2b26-984e-56b4-aa25-b3bd28ea5ac2
> </pre>

### Generate a v5 SHA-1 namespace + name hashed GUID using a custom namespace and the name _`bojangles`_
```bash
idgen v5 bojangles 11de2b26-984e-56b4-aa25-b3bd28ea5ac2
```
> <pre>
> de9425a4-e8dd-510b-8e00-b6ac890c733a
> </pre>

### Generate a v5 SHA-1 namespace + name hashed GUID from `stdin`

```bash
idgen v5 - < /usr/share/dict/words
```
> <pre>
> e54b4418-5a27-5bdb-a1e8-24a89e2a8634
> </pre>

### Generate an ID suitable for an Xcode storyboard
```bash
idgen xcode
```
> <pre>
> KoW-8m-wjo
> </pre>

### Encode a string suitable for [3x4 numeric keypad/phone entry][phonewords]
```bash
idgen phone "hello world"
```
> <pre>
> 43556*96753#
> </pre>

#### GUID Formats
For GUID/UUIDs, a number of representation formats are supported via the `-f` or `-format`

| `-format` | Description |
| --------------- | ----------- |
| `Base64` | The binary representation of the GUID encoded in base 64. _This format ignores the `-upper` option if specified._ |
| `Short` | Like Base64, but with padding (`==`) stripped, `/` changed to `_`, and `+` changed to `-`. |
| `N` | 32 digits:<br>`00000000000000000000000000000000` |
| `D` | 32 digits separated by hyphens:<br>`00000000-0000-0000-0000-000000000000` |
| `B` | 32 digits separated by hyphens, enclosed in braces:<br>`{00000000-0000-0000-0000-000000000000}` |
| `P` | 32 digits separated by hyphens, enclosed in parentheses:<br>`(00000000-0000-0000-0000-000000000000)` |
| `X` | Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces:<br>`{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}` |

[phonewords]: https://en.wikipedia.org/wiki/Telephone_keypad