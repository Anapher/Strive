using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonPatchGenerator.Tests
{
    public class JsonPatchFactoryTests
    {
        [Fact]
        public void TestCreatePatchFlatObject()
        {
            // arrange
            var obj = new TestClass1 {Prop1 = "Hallo Welt", Prop2 = 34, Prop3 = false};
            var newObj = new TestClass1 {Prop1 = "Hello World", Prop2 = 34, Prop3 = false};

            // act
            var patch = JsonPatchFactory.CreatePatch(obj, newObj);

            // assert
            var op = Assert.Single(patch.Operations);
            Assert.Equal(OperationType.Add, op.OperationType);
            Assert.Equal("/Prop1", op.path);
            Assert.Equal("Hello World", op.value);
        }

        [Fact]
        public void TestCreatePatchFlatObjectMultipleValues()
        {
            // arrange
            var obj = new TestClass1 {Prop1 = "Hallo Welt", Prop2 = 34, Prop3 = false};
            var newObj = new TestClass1 {Prop1 = "Hallo Welt", Prop2 = 43, Prop3 = true};

            // act
            var patch = JsonPatchFactory.CreatePatch(obj, newObj);

            // assert
            Assert.Collection(patch.Operations.OrderBy(x => x.path), op =>
            {
                Assert.Equal(OperationType.Add, op.OperationType);
                Assert.Equal("/Prop2", op.path);
                Assert.Equal(43, op.value);
            }, op =>
            {
                Assert.Equal(OperationType.Add, op.OperationType);
                Assert.Equal("/Prop3", op.path);
                Assert.Equal(true, op.value);
            });
        }

        [Fact]
        public void TestCreatePatchDeepObjectMultipleValues()
        {
            // arrange
            var obj = new TestClass2
                {Prop1 = new TestClass1 {Prop1 = "Hallo Welt", Prop2 = 34, Prop3 = false}, Prop2 = "asd"};
            var newObj = new TestClass2
                {Prop1 = new TestClass1 {Prop1 = "Hello Welt", Prop2 = 34, Prop3 = false}, Prop2 = "das"};

            // act
            var patch = JsonPatchFactory.CreatePatch(obj, newObj);

            // assert
            Assert.Collection(patch.Operations.OrderBy(x => x.path), op =>
            {
                Assert.Equal(OperationType.Add, op.OperationType);
                Assert.Equal("/Prop1/Prop1", op.path);
                Assert.Equal("Hello Welt", op.value);
            }, op =>
            {
                Assert.Equal(OperationType.Add, op.OperationType);
                Assert.Equal("/Prop2", op.path);
                Assert.Equal("das", op.value);
            });
        }

        [Fact]
        public void TestCreatePatchFlatListAddItem()
        {
            TestPatch(new TestClass3 {Prop1 = new List<SimpleObj>()},
                new TestClass3 {Prop1 = new List<SimpleObj> {new SimpleObj {Value = "Hello"}}},
                patch => patch.Add("/Prop1/-", new SimpleObj {Value = "Hello"})
            );
        }

        [Fact]
        public void TestCreatePatchFlatListRemoveItem()
        {
            TestPatch(new TestClass3
                {
                    Prop1 = new List<SimpleObj>
                    {
                        new SimpleObj {Value = "Corona"},
                        new SimpleObj {Value = "Pls dont kill me"} // remove
                    }
                },
                new TestClass3
                {
                    Prop1 = new List<SimpleObj> {new SimpleObj {Value = "Corona"}}
                }, patch => patch.Remove("/Prop1/1"));
        }

        [Fact]
        public void TestCreatePatchFlatListPatchItem()
        {
            TestPatch(new TestClass3
            {
                Prop1 = new List<SimpleObj>
                {
                    new SimpleObj {Value = "Corona"},
                    new SimpleObj {Value = "Pls dont kill me"}
                }
            }, new TestClass3
            {
                Prop1 = new List<SimpleObj>
                {
                    new SimpleObj {Value = "Corona"},
                    new SimpleObj {Value = "Plx dont kill me"}
                }
            }, patch => patch.Replace("/Prop1/1", new SimpleObj {Value = "Plx dont kill me"}));
        }

        [Fact]
        public void TestCreatePatchFlatListPatchItemAndChangeLast()
        {
            TestPatch(new TestClass3
            {
                Prop1 = new List<SimpleObj>
                {
                    new SimpleObj {Value = "Corona"},
                    new SimpleObj {Value = "Pls dont kill me"}
                }
            }, new TestClass3
            {
                Prop1 = new List<SimpleObj>
                {
                    new SimpleObj {Value = "Corona"},
                    new SimpleObj {Value = "Plx dont kill me"},
                    new SimpleObj {Value = "it's me"}
                }
            }, patch =>
            {
                patch.Replace("/Prop1/1", new SimpleObj {Value = "Plx dont kill me"});
                patch.Add("/Prop1/-", new SimpleObj {Value = "it's me"});
            });
        }

        [Fact]
        public void TestPatchStringListAddItemToEmptyList()
        {
            TestPatch(new List<string>(),
                new List<string> {"Vincent"},
                patch => patch.Add("/-", "Vincent"));
        }

        [Fact]
        public void TestPatchStringListAddItemToEndOfList()
        {
            TestPatch(new List<string> {"Niklas", "Leo"},
                new List<string> {"Niklas", "Leo", "Vincent"},
                patch => patch.Add("/-", "Vincent"));
        }

        [Fact]
        public void TestPatchStringListInsertItem()
        {
            TestPatch(new List<string> {"Adam", "Eva", "Vincent", "Covid"},
                new List<string> {"Adam", "Eva", "Niklas", "Vincent", "Covid"},
                patch => patch.Add("/2", "Niklas"));
        }

        [Fact]
        public void TestPatchStringListRemoveItem()
        {
            TestPatch(new List<string> {"Adam", "Eva", "Vincent", "Covid"},
                new List<string> {"Adam", "Vincent", "Covid"},
                patch => patch.Remove("/1"));
        }

        [Fact]
        public void TestPatchStringListMoveItem()
        {
            TestPatch(new List<string> {"Adam", "Eva", "Vincent", "Covid"},
                new List<string> {"Eva", "Vincent", "Covid", "Adam"},
                patch =>
                {
                    patch.Move("/1", "/0");
                    patch.Move("/2", "/1");
                    patch.Move("/3", "/2");
                });
        }

        [Fact]
        public void TestPatchStringListInsertAndRemoveItem()
        {
            TestPatch(new List<string> {"Adam", "Eva", "Vincent", "Covid"},
                new List<string> {"Teufel", "Adam", "Vincent", "Covid"},
                patch =>
                {
                    patch.Remove("/1");
                    patch.Add("/0", "Teufel");
                });
        }

        [Fact]
        public void TestPatchDictionaryAddItem()
        {
            TestPatch(new Dictionary<string, string>(), new Dictionary<string, string> {{"hello", "world"}},
                patch => { patch.Add("hello", "world"); });
        }

        [Fact]
        public void TestPatchDictionaryRemoveItem()
        {
            TestPatch(new Dictionary<string, string> {{"hello", "world"}},
                new Dictionary<string, string>(),
                patch => { patch.Remove("hello"); });
        }

        [Fact]
        public void TestPatchDictionaryChangeItem()
        {
            TestPatch(new Dictionary<string, string> {{"hello", "world"}},
                new Dictionary<string, string> {{"hello", "welt"}}, patch => { patch.Add("hello", "welt"); });
        }

        [Fact]
        public void TestPatchDifferentObjectTypesBothDictionary()
        {
            TestPatch(new Dictionary<string, int>(),
                new Dictionary<string, int> {{"hello", 324}}.ToImmutableDictionary(),
                document => document.Add("/hello", "324"));
        }

        private void TestPatch(object obj, object update, Action<JsonPatchDocument> patchCreator)
        {
            // act
            var patch = JsonPatchFactory.CreatePatch(obj, update);

            // assert
            var expectedDoc = new JsonPatchDocument();
            patchCreator(expectedDoc);

            static IEnumerable<Operation> UnifyOpOrder(IEnumerable<Operation> operations)
            {
                return operations.OrderBy(x => x.path).ThenBy(x => x.op);
            }

            Assert.Collection(UnifyOpOrder(patch.Operations),
                UnifyOpOrder(expectedDoc.Operations).Select(expected => new Action<Operation>(actual =>
                {
                    Assert.Equal(expected.OperationType, actual.OperationType);
                    Assert.Equal(expected.path, actual.path);

                    if (expected.value == null)
                        Assert.Null(actual.value);
                    else
                        Assert.Equal(JToken.FromObject(expected.value)?.ToString(),
                            JToken.FromObject(actual.value)?.ToString());
                })).ToArray());
        }

        public class SimpleObj
        {
            public string Value { get; set; }
        }

        public class TestClass1
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public bool Prop3 { get; set; }
        }

        public class TestClass2
        {
            public TestClass1 Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        public class TestClass3
        {
            public List<SimpleObj> Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        public class TestClass4
        {
            public List<TestClass1> Prop1 { get; set; }
        }
    }
}