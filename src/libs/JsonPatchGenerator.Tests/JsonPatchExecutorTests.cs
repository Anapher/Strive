//using System.Collections.Generic;
//using Microsoft.AspNetCore.JsonPatch;
//using Newtonsoft.Json.Linq;
//using Xunit;

//namespace JsonPatchGenerator.Tests
//{
//    public class JsonPatchExecutorTests
//    {
//        public class TestClass1
//        {
//            public string Prop1 { get; set; }
//            public int Prop2 { get; set; }
//            public bool Prop3 { get; set; }
//        }

//        public class TestClass2
//        {
//            public TestClass1 Prop1 { get; set; }
//            public string Prop2 { get; set; }
//        }

//        public class KeyedObject
//        {
//            public string Key { get; set; }
//            public string Value { get; set; }
//        }

//        public class TestClass3
//        {
//            public List<KeyedObject> Prop1 { get; set; }
//            public string Prop2 { get; set; }
//        }

//        public class TestClass4
//        {
//            public List<TestClass1> Prop1 { get; set; }
//        }

//        [Fact]
//        public void TestExecuteFlatPatch()
//        {
//            // arrange
//            var patch = new JsonPatchDocument();
//            patch.Replace("/Prop1", JToken.FromObject("Hello World"));

//            var obj = new TestClass1 {Prop1 = "Hallo Welt", Prop2 = 34, Prop3 = false};

//            // act
//            JsonPatchExecutor.ApplyPatch(patch, obj);

//            // assert
//            Assert.Equal("Hello World", obj.Prop1);
//            Assert.Equal(34, obj.Prop2);
//            Assert.False(obj.Prop3);
//        }

//        [Fact]
//        public void TestExecutePatchFlatObjectMultipleValues()
//        {
//            // arrange
//            var patch = new JsonPatchDocument();
//            patch.Replace("/Prop2", JToken.FromObject(45));
//            patch.Replace("/Prop3", JToken.FromObject(true));

//            var obj = new TestClass1 { Prop1 = "Hallo Welt", Prop2 = 34, Prop3 = false };

//            // act
//            JsonPatchExecutor.ApplyPatch(patch, obj);

//            // assert
//            Assert.Equal("Hallo Welt", obj.Prop1);
//            Assert.Equal(45, obj.Prop2);
//            Assert.True(obj.Prop3);
//        }

//        [Fact]
//        public void TestExecutePatchDeepObjectMultipleValues()
//        {
//            // arrange
//            var patch = new JsonPatchDocument();
//            patch.Replace("/Prop1/Prop1", JToken.FromObject("wtf"));
//            patch.Replace("/Prop2", JToken.FromObject("das"));

//            var obj = new TestClass2 { Prop1 = new TestClass1 { Prop1 = "Hallo Welt", Prop2 = 34, Prop3 = false }, Prop2 = "asd" };

//            // act
//            JsonPatchExecutor.ApplyPatch(patch, obj);

//            // assert
//            Assert.Equal("wtf", obj.Prop1.Prop1);
//            Assert.Equal("das", obj.Prop2);
//        }

//        [Fact]
//        public void TestExecutePatchFlatListAddItem()
//        {
//            // arrange
//            var patch = new JsonPatchDocument();
//            patch.Add("/Prop1", JToken.FromObject(new KeyedObject{Key = "2", Value = "test"}));

//            var obj = new TestClass3 {Prop1 = new List<KeyedObject>()};

//            // act
//            JsonPatchExecutor.ApplyPatch(patch, obj);

//            // assert
//            var item = Assert.Single(obj.Prop1);
//            Assert.Equal("2", item.Key);
//            Assert.Equal("test", item.Value);
//        }

//        [Fact]
//        public void TestCreatePatchFlatListRemoveItem()
//        {
//            // arrange
//            var patch = new JsonPatchDocument();
//            patch.Remove("/Prop1/2");

//            var obj = new TestClass3 {Prop1 = new List<KeyedObject> {new KeyedObject {Key = "2", Value = "Hallo Welt"}}};

//            // act
//            JsonPatchExecutor.ApplyPatch(patch, obj);

//            // assert
//            Assert.Empty(obj.Prop1);
//        }

//        [Fact]
//        public void TestCreatePatchFlatListPatchItem()
//        {
//            // arrange
//            var patch = new JsonPatchDocument();
//            patch.Replace("/Prop1/2/Value", JToken.FromObject("Hello World"));

//            var obj = new TestClass3 { Prop1 = new List<KeyedObject> { new KeyedObject { Key = "2", Value = "Hallo Welt" } } };

//            // act
//            JsonPatchExecutor.ApplyPatch(patch, obj);

//            // assert
//            var item = Assert.Single(obj.Prop1);
//            Assert.Equal("2", item.Key);
//            Assert.Equal("Hello World", item.Value);
//        }
//    }
//}

