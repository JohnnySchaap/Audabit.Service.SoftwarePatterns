namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging;

public interface IMessagingPatternsService
{
    void Outbox();

    void Inbox();

    void Splitter();

    void Aggregator();

    void MessageRouter();

    void ContentBasedRouter();

    void MessageTranslator();

    void PublishSubscribe();

    void RequestReply();

    void DeadLetterChannel();
}