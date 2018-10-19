# Identifier Generator

![][travislinuxbs]

[travislinuxbs]: https://travis-ci.org/abock/idgen.svg?branch=master "Travis: Linux Build Status"

Generate some IDs!

## `--help`

```
Usage: idgen [OPTIONS]+

OPTIONS:

  -h, -?, --help             Show this help.
  -n=NUMBER                  Generate NUMBER of identifiers
  -u, --upper                Output the identifier in upper-case
  -f, --format=FORMAT        FORMAT to use (see FORMATS) for GUID identifiers.
      --guidv4               Generate a random version 4 GUID (default ID type
                               if no other type options are specified).
      --guidv5=VALUE         Generate a version 5 GUID based on a SHA-1 hash of
                               VALUE and a namespace provided by the -namespace
                               option. The URL namespace will be used if one is
                               not specified.
      --guidv3=VALUE         Generate a version 3 GUID based on a MD5 hash of
                               VALUE and a namespace provided by the -namespace
                               option. The URL namespace will be used if one is
                               not specified.
      --namespace=NAMESPACE_GUID
                             For -guidv5 and -guidv3 options, specify the
                               NAMESPACE_GUID that should be used. The
                               namespace must itself be a GUID.
      --xcode                Generate an identifier for use in Xcode
                               storyboards/XIBs.
  -b, --bojangles            Ensures generated identifiers will have with all '
                               AA' strings replaced with 'BB' strings.

FORMATS:

  Base64   The binary representation of the GUID encoded in base 64.
           This format ignores the -uppercase option if specified.

  N        32 digits:
           00000000000000000000000000000000

  D        32 digits separated by hyphens:
           00000000-0000-0000-0000-000000000000

  B        32 digits separated by hyphens, enclosed in braces:
           {00000000-0000-0000-0000-000000000000}

  P        32 digits separated by hyphens, enclosed in parentheses:
           (00000000-0000-0000-0000-000000000000)

  X        Four hexadecimal values enclosed in braces, where the
           fourth value is a subset of eight hexadecimal values
           that is also enclosed in braces:
           {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
```