namespace AutoTravel.GameSystems.Steps;

internal interface IStepActions
{
    void OnSuccess();

    void OnFail(string message);
}
