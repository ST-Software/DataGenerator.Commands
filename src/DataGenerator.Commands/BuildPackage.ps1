dnu restore
dnu build
if (Test-Path .\package) {
	rm .\package -recurse
}
dnu pack --out .\package