﻿using Content.Client.UserInterface.Controls;
using Content.Shared._Sunrise.Boss.Systems;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;

namespace Content.Client._Sunrise.Boss.UI;

[GenerateTypedNameReferences]
public sealed partial class HellSpawnArenaConsoleWindow : FancyWindow
{
    private readonly IGameTiming _timing;

    public TimeSpan? ActivationTime;
    public HellSpawnBossStatus State = HellSpawnBossStatus.Idle;

    public HellSpawnArenaConsoleWindow()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);

        _timing = IoCManager.Resolve<IGameTiming>();

        TravelButton.OnPressed += _ => TravelButtonPressed?.Invoke();

        StatusLabel.Text = Loc.GetString("hellspawn-arena-console-status-idle");
    }

    public event Action? TravelButtonPressed;

    public void UpdateState(HellSpawnArenaConsoleUiState state)
    {
        // TravelButton.Disabled = state.Status == HellSpawnBossStatus.InProgress;

        if (state.ActivationTime != null)
            ActivationTime = state.ActivationTime;

        State = state.CurrentStatus;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        StatusLabel.Text = State switch
        {
            HellSpawnBossStatus.Idle => Loc.GetString("hellspawn-arena-console-status-idle"),
            HellSpawnBossStatus.Cooldown => Loc.GetString("hellspawn-arena-console-status-cooldown"),
            HellSpawnBossStatus.InProgress => Loc.GetString("hellspawn-arena-console-status-inprogress"),
            _ => StatusLabel.Text,
        };
        switch (State)
        {
            case HellSpawnBossStatus.Idle:
                if (ActivationTime != null && ActivationTime.Value > _timing.CurTime)
                {
                    TravelButton.Text = $"{(ActivationTime.Value - _timing.CurTime).TotalSeconds.ToString()}";
                    TravelButton.Disabled = true;
                }
                else
                {
                    TravelButton.Text = Loc.GetString("hellspawn-arena-console-ready");
                    TravelButton.Disabled = false;
                }

                break;
            case HellSpawnBossStatus.Cooldown:
                if (ActivationTime != null && ActivationTime.Value > _timing.CurTime)
                {
                    TravelButton.Text = $"{(ActivationTime.Value - _timing.CurTime).TotalSeconds.ToString()}";
                    TravelButton.Disabled = true;
                }
                else
                {
                    TravelButton.Text = Loc.GetString("hellspawn-arena-console-ready");
                    TravelButton.Disabled = false;
                }

                break;
            case HellSpawnBossStatus.InProgress:
                break;
        }
    }
}
