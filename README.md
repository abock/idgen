# Identifier Generator

[![Travis CI](https://travis-ci.org/abock/idgen.svg?branch=master)](https://travis-ci.org/abock/idgen)
[![NuGet Badge](https://buildstats.info/nuget/IdentifierGenerator)](https://www.nuget.org/packages/IdentifierGenerator/)
[![License](https://img.shields.io/badge/license-MIT%20License-blue.svg)](LICENSE)

`idgen` is a .NET Core global tool that supports the bulk generation of various types of unique identifiers:

* [Versions 3 and 5 GUID/UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier#Versions_3_and_5_(namespace_name-based)): reproducible hashed namespace + name IDs
  * _Note: use version 5 (SHA-1) over version 3 (MD5) unless compatibility is required._
* [Version 4 GUID/UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier#Version_4_(random)): secure random IDs
* [Nano ID](https://zelark.github.io/nano-id-cc/): secure random IDs of configurable size and alphabet
* Xcode Storyboard/XIB IDs

## Install & Update

### Install

```bash
$ dotnet tool install -g IdentifierGenerator
```

### Update

```bash
$ dotnet tool update -g IdentifierGenerator
```

## Examples

Run `idgen --help` after installing for detailed options.

### Generate a single v4 random GUID
```bash
$ idgen
0b0d5b33-b5e9-45cb-8f14-9bdab594cc98
```

### Do the same, but upper-case it
```bash
$ idgen -upper
8E350BC7-FF37-4E96-A5F7-CD945C9BDC33
```

### Generate a true base-64 encoded v4 random GUID
```bash
$ idgen -f base64
JWn2giJJhUePnVzrCAK8JQ==
```

### Generate a true short v4 random GUID (base-64 with minor transformations)
```bash
$ idgen -f short
9lsQ5-h1nEy9uS3DMbLoeg
```

### Generate 5 Nano IDs
```bash
$ idgen -n 5 -nanoid
Fm82ZL3eyabMeVAgDGF7k
LlrnWI3YrhUbQY3zHiyYc
JUExm8eTVmLjLBjVeabZd
1bNIDlndN6W~chHMDq2y9
izaokjb4E9ft6~rAgINEy
```

### Generate a Nano ID with a custom size and alphabet
```bash
$ idgen -nanoid=32 -nanoid-alphabet=abcdefghijklmnopqrstuvwxyz
aqmtbhpgomnpvudpmtesoooakyrrdrap
```

### Generate a v5 SHA-1 namespace + name hashed GUID using the URL namespace and the name _`bojangles`_
```bash
$ idgen -guidv5 bojangles
11de2b26-984e-56b4-aa25-b3bd28ea5ac2
```

### Generate a v5 SHA-1 namespace + name hashed GUID using a custom namespace and the name _`bojangles`_
```bash
$ idgen -guidv5 bojangles -namespace 11de2b26-984e-56b4-aa25-b3bd28ea5ac2
de9425a4-e8dd-510b-8e00-b6ac890c733a
```

### Generate an ID suitable for an Xcode storyboard
```bash
$ idgen -xcode
KoW-8m-wjo
```

#### GUID Formats

For GUID/UUIDs, a number of representation formats are supported via the `-f` or `-format`

| `-format` | Description |
| --------------- | ----------- |
| `Base64` | The binary representation of the GUID encoded in base 64. _This format ignores the `-upper` option if specified._ |
| `Short` | Like Base64, but with padding (`==`) stripped, `/` changed to `_`, and `+` changed to `-`. |
| `N` | 32 digits:<br>`00000000000000000000000000000000` |
| `D` | 32 digits separated by hyphens:<br>`00000000-0000-0000-0000-000000000000)` |
| `B` | 32 digits separated by hyphens, enclosed in braces:<br>`{00000000-0000-0000-0000-000000000000}` |
| `P` | 32 digits separated by hyphens, enclosed in parentheses:<br>`(00000000-0000-0000-0000-000000000000)` |
| `X` | Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces:<br>`{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}` |
