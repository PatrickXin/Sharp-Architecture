#pragma warning disable 169
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace SharpArch.Specifications.SharpArch.Core
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    using global::SharpArch.Core;
    using Machine.Specifications;

    public class retreiving_an_object_from_the_file_cache
    {
        [Subject("Retreiving an object from the file cache")]
        public class when_specifying_an_empty_file_path
        {
            static Exception exception;

            Because of = () => exception = Catch.Exception(() => FileCache.RetrieveFromCache<string>(string.Empty));

            It should_throw_an_argument_exception = () =>
                exception.ShouldBeOfType(typeof(ArgumentException));
        }

        [Subject("Retreiving an object from the file cache")]
        public class when_specifying_a_null_file_path
        {
            static Exception exception;

            Because of = () => exception = Catch.Exception(() => FileCache.RetrieveFromCache<string>(null));

            It should_throw_an_argument_exception = () =>
                exception.ShouldBeOfType(typeof(ArgumentException));
        }

        [Subject("Retreiving an object from the file cache")]
        public class when_the_file_contents_are_invalid
        {
            static object result;

            Establish context = () => File.WriteAllText("dummy.txt", "&&&");

            Because of = () => result = FileCache.RetrieveFromCache<DummyType>("dummy.txt");

            It should_return_null = () => result.ShouldBeNull();

            Cleanup after = () => { if (File.Exists("dummy.txt")) File.Delete("dummy.txt"); };
        }

        [Subject("Retreiving an object from the file cache")]
        public class when_the_file_contents_are_valid
        {
            static object result;
            static DummyType dummy = new DummyType { FirstProperty = "First" };

            Establish context = () =>
                {
                    using (FileStream file = File.Open("dummy.txt", FileMode.Create)) {
                        new BinaryFormatter().Serialize(file, dummy);
                    }
                };

            Because of = () => result = FileCache.RetrieveFromCache<DummyType>("dummy.txt");

            It should_return_the_correct_type_of_object = () =>
                result.ShouldBeOfType(typeof(DummyType));

            It should_set_the_objects_properties_correctly = () =>
                ((DummyType)result).FirstProperty.ShouldEqual("First");

            Cleanup after = () => { if (File.Exists("dummy.txt")) File.Delete("dummy.txt"); };
        }
    }

    public class storing_an_object_in_the_file_cache
    {
        [Subject("Storing an object in the file cache")]
        public class when_trying_to_store_a_null_object
        {
            static Exception exception;

            Because of = () => exception = Catch.Exception(() => FileCache.StoreInCache<DummyType>(null, "path.txt"));

            It should_throw_an_argument_exception = () =>
                exception.ShouldBeOfType(typeof(ArgumentException));
        }

        [Subject("Storing an object in the file cache")]
        public class when_specifying_an_empty_file_path
        {
            static Exception exception;
            static DummyType dummy = new DummyType();

            Because of = () => exception = Catch.Exception(() => FileCache.StoreInCache<DummyType>(dummy, string.Empty));

            It should_throw_an_argument_exception = () =>
                exception.ShouldBeOfType(typeof(ArgumentException));
        }

        [Subject("Storing an object in the file cache")]
        public class when_specifying_a_valid_object
        {
            static DummyType dummy = new DummyType { FirstProperty = "First" };

            Because of = () => FileCache.StoreInCache<DummyType>(dummy, "dummy.txt");

            It should_serialize_the_object_to_the_file = () => File.Exists("dummy.txt").ShouldBeTrue();

            Cleanup after = () => { if (File.Exists("dummy.txt")) File.Delete("dummy.txt"); };
        }
    }

    [Serializable]
    public class DummyType
    {
        public string FirstProperty { get; set; }
    }
}