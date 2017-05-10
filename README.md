# httpclient-tests

Used with: https://github.com/gortok/webservice-test to test out HttpClient out of the box in .NET Core.

Pull each repo down, and from their parent directory:
```
pushd ./webservice-test 
sh build.sh
popd
pushd ./httpclient-tests
sh build.sh
popd
docker logs -f httpclientsync
```
