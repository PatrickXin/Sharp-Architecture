#pragma warning disable 169
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace SharpArch.Specifications.SharpArch.Core
{
    using System;
    using System.Diagnostics;

    using global::SharpArch.Core;

    using Machine.Specifications;

    using Rhino.Mocks;

    public class specification_for_design_by_contract
    {
        protected static string ConvertToPercentage(decimal fractionalPercentage)
        {
            Check.Require(fractionalPercentage > 0,
                "fractionalPercentage must be > 0");

            decimal convertedValue = fractionalPercentage * 100;

            // Yes, I could have enforced this outcome in the precondition, but then you
            // wouldn't have seen the Check.Ensure in action, now would you?
            Check.Ensure(convertedValue >= 0 && convertedValue <= 100,
                "convertedValue was expected to be within 0-100, but was " + convertedValue);

            return Math.Round(convertedValue) + "%";
        }

    }

    [Subject(typeof(Check))]
    public class when_a_method_using_a_precondition_is_called_using_a_valid_argument : specification_for_design_by_contract
    {
        static Exception result;

        Establish context = () => Check.UseAssertions = false;

        Because of = () => result = Catch.Exception(() => ConvertToPercentage(0.2m));

        It should_not_throw_a_precondition_exception = () => result.ShouldBeNull();
    }

    [Subject(typeof(Check))]
    public class when_a_method_using_a_precondition_is_called_using_an_invalid_argument : specification_for_design_by_contract
    {
        static Exception result;

        Establish context = () => Check.UseAssertions = false;

        Because of = () => result = Catch.Exception(() => ConvertToPercentage(-0.2m));

        It should_throw_a_precondition_exception = () => result.ShouldBeOfType<PreconditionException>();
    }

    [Subject(typeof(Check))]
    public class when_a_method_using_a_postcondition_results_in_an_invalid_return_value : specification_for_design_by_contract
    {
        static Exception result;

        Establish context = () => Check.UseAssertions = false;

        Because of = () => result = Catch.Exception(() => ConvertToPercentage(10m));

        It should_throw_a_postcondition_exception = () => result.ShouldBeOfType<PostconditionException>();
    }

    [Subject(typeof(Check))]
    public class when_a_method_using_a_precondition_is_called_using_an_invalid_argument_and_use_assertions_has_been_set_to_true : specification_for_design_by_contract
    {
        static Exception result;

        static TraceListener traceListener;

        Establish context = () =>
            {
                Check.UseAssertions = true;
                traceListener = MockRepository.GenerateMock<TraceListener>();
                traceListener.Stub(l => l.Fail(Arg<string>.Is.Anything));
                Trace.Listeners.Clear();
                Trace.Listeners.Add(traceListener);
            };

        Because of = () => result = Catch.Exception(() => ConvertToPercentage(-0.2m));

        It should_not_throw_an_exception = () => result.ShouldBeNull();

        It should_call_the_fail_method_on_the_trace_listener_for_precondition_and_postcondition_violations = () => traceListener.AssertWasCalled(l => l.Fail(Arg<string>.Is.Anything), o => o.Repeat.Twice());
    }

    [Subject(typeof(Check))]
    public class when_a_method_using_a_postcondition_results_in_an_invalid_return_value_and_use_assertions_has_been_set_to_true : specification_for_design_by_contract
    {
        static Exception result;

        static TraceListener traceListener;

        Establish context = () =>
        {
            Check.UseAssertions = true;
            traceListener = MockRepository.GenerateMock<TraceListener>();
            Trace.Listeners.Clear();
            Trace.Listeners.Add(traceListener);
        };

        Because of = () => result = Catch.Exception(() => ConvertToPercentage(10m));

        It should_not_throw_a_postcondition_exception = () => result.ShouldBeNull();

        It should_call_the_fail_method_on_the_trace_listener = () => traceListener.AssertWasCalled(l => l.Fail(Arg<string>.Is.Anything));
    }
}