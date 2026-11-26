class OptionWnd extends UIScript;

var int nPixelShaderVersion;
var int nVertexShaderVersion;
var float gSoundVolume;
var float gMusicVolume;
var float gWavVoiceVolume;
var float gOggVoiceVolume;
var array<ResolutionInfo> ResolutionList;
var array<int> RefreshRateList;
var bool bShow;
var int m_iPrevSoundTick;
var int m_iPrevMusicTick;
var int m_iPrevSystemTick;
var int m_iPrevTutorialTick;
var bool m_bPartyMatchRoomState;

function ResetRefreshRate(optional int a_nWidth, optional int a_nHeight)
{
	local int i;

	GetRefreshRateList(RefreshRateList, a_nWidth, a_nHeight);
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("OptionWnd.RefreshRateBox");
	i = 0;
	J0x45:

	// End:0xC7 [Loop If]
	if(__NFUN_150__(i, RefreshRateList.Length))
	{
		Debug(__NFUN_112__("RefreshRateList[ i ] ", string(RefreshRateList[i])));
		Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.RefreshRateBox", __NFUN_112__(string(RefreshRateList[i]), "Hz"));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x45;
	}
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.RefreshRateBox", __NFUN_147__(i, 1));
	return;
}

function OnLoad()
{
	local int i, nMultiSample;
	local bool bEnableEngSelection;
	local UIEventManager.ELanguageType Language;
	local string strResolution;

	RegisterEvent(510);
	RegisterEvent(520);
	RegisterEvent(1550);
	RegisterEvent(1560);
	RegisterState("OptionWnd", "GamingState");
	RegisterState("OptionWnd", "LoginState");
	GetShaderVersion(nPixelShaderVersion, nVertexShaderVersion);
	GetResolutionList(ResolutionList);
	SetOptionBool("Game", "HideDropItem", false);
	i = 0;
	J0xA4:

	// End:0x139 [Loop If]
	if(__NFUN_150__(i, ResolutionList.Length))
	{
		strResolution = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", string(ResolutionList[i].nWidth)), "*"), string(ResolutionList[i].nHeight)), " "), string(ResolutionList[i].nColorBit)), "bit");
		Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.ResBox", strResolution);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xA4;
	}
	ResetRefreshRate();
	nMultiSample = GetMultiSample();
	// End:0x19E
	if(__NFUN_154__(0, nMultiSample))
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.AABox", 869);
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.AABox");		
	}
	else
	{
		// End:0x216
		if(__NFUN_154__(1, nMultiSample))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.AABox", 869);
			Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.AABox", 870);
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.AABox");			
		}
		else
		{
			// End:0x2B1
			if(__NFUN_154__(2, nMultiSample))
			{
				Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.AABox", 869);
				Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.AABox", 870);
				Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.AABox", 871);
				Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.AABox");
			}
		}
	}
	bEnableEngSelection = IsEnableEngSelection();
	Language = GetLanguage();
	switch(Language)
	{
		// End:0x2D9
		case 0:
			// End:0x6CC
			break;
		// End:0x396
		case 1:
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "Korean");
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "English");
			// End:0x36D
			if(bEnableEngSelection)
			{
				Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.LanguageBox");				
			}
			else
			{
				Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.LanguageBox");
			}
			// End:0x6CC
			break;
		// End:0x3CD
		case 2:
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "English");
			// End:0x6CC
			break;
		// End:0x48C
		case 3:
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "Japanese");
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "English");
			// End:0x463
			if(bEnableEngSelection)
			{
				Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.LanguageBox");				
			}
			else
			{
				Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.LanguageBox");
			}
			// End:0x6CC
			break;
		// End:0x552
		case 4:
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "Chinese(Taiwan)");
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "English");
			// End:0x529
			if(bEnableEngSelection)
			{
				Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.LanguageBox");				
			}
			else
			{
				Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.LanguageBox");
			}
			// End:0x6CC
			break;
		// End:0x60E
		case 5:
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "China");
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "English");
			// End:0x5E5
			if(bEnableEngSelection)
			{
				Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.LanguageBox");				
			}
			else
			{
				Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.LanguageBox");
			}
			// End:0x6CC
			break;
		// End:0x6C9
		case 6:
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "Thai");
			Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.LanguageBox", "English");
			// End:0x6A0
			if(bEnableEngSelection)
			{
				Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.LanguageBox");				
			}
			else
			{
				Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.LanguageBox");
			}
			// End:0x6CC
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0x747
	if(CanUseHDR())
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.HDRBox", 1230);
		Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.HDRBox", 1231);
		Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddString("OptionWnd.HDRBox", 1232);
	}
	InitVideoOption();
	InitAudioOption();
	InitGameOption();
	bShow = false;
	return;
}

function RefreshLootingBox()
{
	// End:0x3F
	if(__NFUN_132__(__NFUN_151__(GetPartyMemberCount(), 0), m_bPartyMatchRoomState))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.LootingBox");		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.LootingBox");
	}
	return;
}

function InitVideoOption()
{
	local int i, nResolutionIndex;
	local float fGamma;
	local bool bRenderDeco;
	local int nPostProcessType;
	local bool bShadow;
	local int nTextureDetail, nModelDetail, nSkipAnim;
	local bool bWaterEffect;
	local int nWaterEffectType, nRenderActorLimit, nMultiSample, nOption;
	local bool bOption;

	nResolutionIndex = GetResolutionIndex();
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ResBox", nResolutionIndex);
	GetRefreshRateList(RefreshRateList);
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("OptionWnd.RefreshRateBox");
	i = 0;
	J0x6D:

	// End:0xC3 [Loop If]
	if(__NFUN_150__(i, RefreshRateList.Length))
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.AddString("OptionWnd.RefreshRateBox", __NFUN_112__(string(RefreshRateList[i]), "Hz"));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x6D;
	}
	nOption = GetOptionInt("Video", "RefreshRate");
	i = 0;
	J0xEA:

	// End:0x181 [Loop If]
	if(__NFUN_150__(i, RefreshRateList.Length))
	{
		Debug(__NFUN_112__(__NFUN_112__(__NFUN_112__("RefreshRateList[ ", string(i)), " ] = "), string(RefreshRateList[i])));
		// End:0x177
		if(__NFUN_154__(RefreshRateList[i], nOption))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.RefreshRateBox", i);
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xEA;
	}
	fGamma = GetOptionFloat("Video", "Gamma");
	// End:0x1D1
	if(__NFUN_178__(1.2000000, fGamma))
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.GammaBox", 0);		
	}
	else
	{
		// End:0x218
		if(__NFUN_130__(__NFUN_178__(1.0000000, fGamma), __NFUN_176__(fGamma, 1.2000000)))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.GammaBox", 1);			
		}
		else
		{
			// End:0x260
			if(__NFUN_130__(__NFUN_178__(0.8000000, fGamma), __NFUN_176__(fGamma, 1.0000000)))
			{
				Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.GammaBox", 2);				
			}
			else
			{
				// End:0x2A8
				if(__NFUN_130__(__NFUN_178__(0.6000000, fGamma), __NFUN_176__(fGamma, 0.8000000)))
				{
					Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.GammaBox", 3);					
				}
				else
				{
					// End:0x2DC
					if(__NFUN_176__(fGamma, 0.6000000))
					{
						Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.GammaBox", 4);
					}
				}
			}
		}
	}
	nOption = GetOptionInt("Video", "PawnClippingRange");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.CharBox", nOption);
	nOption = GetOptionInt("Video", "TerrainClippingRange");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.TerrainBox", nOption);
	bRenderDeco = GetOptionBool("Video", "RenderDeco");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.DecoBox", bRenderDeco);
	nPostProcessType = GetOptionInt("Video", "PostProc");
	// End:0x423
	if(__NFUN_130__(__NFUN_152__(0, nPostProcessType), __NFUN_152__(nPostProcessType, 5)))
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.HDRBox", nPostProcessType);		
	}
	else
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.HDRBox", 0);
	}
	bShadow = GetOptionBool("Video", "PawnShadow");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.ShadowBox", bShadow);
	nTextureDetail = GetOptionInt("Video", "TextureDetail");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.TextureBox", __NFUN_250__(0, __NFUN_249__(2, nTextureDetail)));
	nModelDetail = GetOptionInt("Video", "ModelDetail");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ModelBox", nModelDetail);
	nSkipAnim = GetOptionInt("Video", "SkipAnim");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.AnimBox", nSkipAnim);
	// End:0x5C8
	if(__NFUN_150__(nPixelShaderVersion, 12))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.ReflectBox");
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ReflectBox", 0);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.ReflectBox");
		bWaterEffect = GetOptionBool("L2WaterEffect", "IsUseEffect");
		nWaterEffectType = GetOptionInt("L2WaterEffect", "EffectType");
		// End:0x671
		if(__NFUN_129__(bWaterEffect))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ReflectBox", 0);			
		}
		else
		{
			// End:0x6A5
			if(__NFUN_154__(nWaterEffectType, 1))
			{
				Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ReflectBox", 1);				
			}
			else
			{
				// End:0x6DB
				if(__NFUN_154__(nWaterEffectType, 2))
				{
					Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ReflectBox", 2);					
				}
				else
				{
					Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ReflectBox", 0);
				}
			}
		}
	}
	bOption = GetOptionBool("Video", "UseTrilinear");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.TriBox", bOption);
	nRenderActorLimit = GetOptionInt("Video", "RenderActorLimited");
	// End:0x7A3
	if(__NFUN_153__(nRenderActorLimit, 6))
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.PawnBox", 0);		
	}
	else
	{
		// End:0x7D5
		if(__NFUN_154__(nRenderActorLimit, 5))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.PawnBox", 1);			
		}
		else
		{
			// End:0x808
			if(__NFUN_154__(nRenderActorLimit, 4))
			{
				Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.PawnBox", 2);				
			}
			else
			{
				// End:0x83B
				if(__NFUN_154__(nRenderActorLimit, 3))
				{
					Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.PawnBox", 3);					
				}
				else
				{
					// End:0x86E
					if(__NFUN_154__(nRenderActorLimit, 2))
					{
						Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.PawnBox", 4);						
					}
					else
					{
						// End:0x89D
						if(__NFUN_152__(nRenderActorLimit, 1))
						{
							Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.PawnBox", 5);
						}
					}
				}
			}
		}
	}
	nMultiSample = GetMultiSample();
	// End:0x93B
	if(__NFUN_130__(__NFUN_151__(nMultiSample, 0), __NFUN_129__(__NFUN_130__(__NFUN_152__(3, nPostProcessType), __NFUN_152__(nPostProcessType, 5)))))
	{
		nOption = GetOptionInt("Video", "AntiAliasing");
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.AABox", nOption);
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.AABox");		
	}
	else
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.AABox", 0);
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.AABox");
	}
	bOption = GetOptionBool("Video", "IsKeepMinFrameRate");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.FrameBox", bOption);
	// End:0x9DF
	if(bOption)
	{
		MinFrameRateOn();		
	}
	else
	{
		MinFrameRateOff();
	}
	nOption = GetOptionInt("Game", "ScreenShotQuality");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.CaptureBox", nOption);
	nOption = GetOptionInt("Video", "WeatherEffect");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.WeatherEffectComboBox", nOption);
	bOption = GetOptionBool("Video", "GPUAnimation");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.GPUAnimationCheckBox", bOption);
	// End:0xB4D
	if(__NFUN_150__(nVertexShaderVersion, 20))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.GPUAnimationCheckBox");
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.GPUAnimationCheckBox", false);
	}
	return;
}

function InitAudioOption()
{
	local float fSoundVolume, fMusicVolume, fWavVoiceVolume, fOggVoiceVolume;
	local int iSoundVolume, iMusicVolume, iSystemVolume, iTutorialVolume;

	// End:0x4B
	if(__NFUN_242__(GetOptionBool("Audio", "AudioMuteOn"), true))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.mutecheckbox", true);		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.mutecheckbox", false);
	}
	// End:0x5E6
	if(CanUseAudio())
	{
		fSoundVolume = GetOptionFloat("Audio", "SoundVolume");
		gSoundVolume = fSoundVolume;
		// End:0xD1
		if(__NFUN_130__(__NFUN_178__(0.0000000, fSoundVolume), __NFUN_176__(fSoundVolume, 0.2000000)))
		{
			iSoundVolume = 0;			
		}
		else
		{
			// End:0xFB
			if(__NFUN_130__(__NFUN_178__(0.2000000, fSoundVolume), __NFUN_176__(fSoundVolume, 0.4000000)))
			{
				iSoundVolume = 1;				
			}
			else
			{
				// End:0x126
				if(__NFUN_130__(__NFUN_178__(0.4000000, fSoundVolume), __NFUN_176__(fSoundVolume, 0.6000000)))
				{
					iSoundVolume = 2;					
				}
				else
				{
					// End:0x151
					if(__NFUN_130__(__NFUN_178__(0.6000000, fSoundVolume), __NFUN_176__(fSoundVolume, 0.8000000)))
					{
						iSoundVolume = 3;						
					}
					else
					{
						// End:0x17C
						if(__NFUN_130__(__NFUN_178__(0.8000000, fSoundVolume), __NFUN_176__(fSoundVolume, 1.0000000)))
						{
							iSoundVolume = 4;							
						}
						else
						{
							// End:0x193
							if(__NFUN_178__(1.0000000, fSoundVolume))
							{
								iSoundVolume = 5;
							}
						}
					}
				}
			}
		}
		Class'NWindow.UIAPI_SLIDERCTRL'.static.SetCurrentTick("OptionWnd.EffectVolumeSliderCtrl", iSoundVolume);
		fMusicVolume = GetOptionFloat("Audio", "MusicVolume");
		gMusicVolume = fMusicVolume;
		// End:0x21E
		if(__NFUN_130__(__NFUN_178__(0.0000000, fMusicVolume), __NFUN_176__(fMusicVolume, 0.2000000)))
		{
			iMusicVolume = 0;			
		}
		else
		{
			// End:0x248
			if(__NFUN_130__(__NFUN_178__(0.2000000, fMusicVolume), __NFUN_176__(fMusicVolume, 0.4000000)))
			{
				iMusicVolume = 1;				
			}
			else
			{
				// End:0x273
				if(__NFUN_130__(__NFUN_178__(0.4000000, fMusicVolume), __NFUN_176__(fMusicVolume, 0.6000000)))
				{
					iMusicVolume = 2;					
				}
				else
				{
					// End:0x29E
					if(__NFUN_130__(__NFUN_178__(0.6000000, fMusicVolume), __NFUN_176__(fMusicVolume, 0.8000000)))
					{
						iMusicVolume = 3;						
					}
					else
					{
						// End:0x2C9
						if(__NFUN_130__(__NFUN_178__(0.8000000, fMusicVolume), __NFUN_176__(fMusicVolume, 1.0000000)))
						{
							iMusicVolume = 4;							
						}
						else
						{
							// End:0x2E0
							if(__NFUN_178__(1.0000000, fMusicVolume))
							{
								iMusicVolume = 5;
							}
						}
					}
				}
			}
		}
		Class'NWindow.UIAPI_SLIDERCTRL'.static.SetCurrentTick("OptionWnd.MusicVolumeSliderCtrl", iMusicVolume);
		fWavVoiceVolume = GetOptionFloat("Audio", "WavVoiceVolume");
		gWavVoiceVolume = fWavVoiceVolume;
		// End:0x36D
		if(__NFUN_130__(__NFUN_178__(0.0000000, fWavVoiceVolume), __NFUN_176__(fWavVoiceVolume, 0.2000000)))
		{
			iSystemVolume = 0;			
		}
		else
		{
			// End:0x397
			if(__NFUN_130__(__NFUN_178__(0.2000000, fWavVoiceVolume), __NFUN_176__(fWavVoiceVolume, 0.4000000)))
			{
				iSystemVolume = 1;				
			}
			else
			{
				// End:0x3C2
				if(__NFUN_130__(__NFUN_178__(0.4000000, fWavVoiceVolume), __NFUN_176__(fWavVoiceVolume, 0.6000000)))
				{
					iSystemVolume = 2;					
				}
				else
				{
					// End:0x3ED
					if(__NFUN_130__(__NFUN_178__(0.6000000, fWavVoiceVolume), __NFUN_176__(fWavVoiceVolume, 0.8000000)))
					{
						iSystemVolume = 3;						
					}
					else
					{
						// End:0x418
						if(__NFUN_130__(__NFUN_178__(0.8000000, fWavVoiceVolume), __NFUN_176__(fWavVoiceVolume, 1.0000000)))
						{
							iSystemVolume = 4;							
						}
						else
						{
							// End:0x42F
							if(__NFUN_178__(1.0000000, fWavVoiceVolume))
							{
								iSystemVolume = 5;
							}
						}
					}
				}
			}
		}
		Class'NWindow.UIAPI_SLIDERCTRL'.static.SetCurrentTick("OptionWnd.SystemVolumeSliderCtrl", iSystemVolume);
		fOggVoiceVolume = GetOptionFloat("Audio", "OggVoiceVolume");
		gOggVoiceVolume = fOggVoiceVolume;
		// End:0x4BD
		if(__NFUN_130__(__NFUN_178__(0.0000000, fOggVoiceVolume), __NFUN_176__(fOggVoiceVolume, 0.2000000)))
		{
			iTutorialVolume = 0;			
		}
		else
		{
			// End:0x4E7
			if(__NFUN_130__(__NFUN_178__(0.2000000, fOggVoiceVolume), __NFUN_176__(fOggVoiceVolume, 0.4000000)))
			{
				iTutorialVolume = 1;				
			}
			else
			{
				// End:0x512
				if(__NFUN_130__(__NFUN_178__(0.4000000, fOggVoiceVolume), __NFUN_176__(fOggVoiceVolume, 0.6000000)))
				{
					iTutorialVolume = 2;					
				}
				else
				{
					// End:0x53D
					if(__NFUN_130__(__NFUN_178__(0.6000000, fOggVoiceVolume), __NFUN_176__(fOggVoiceVolume, 0.8000000)))
					{
						iTutorialVolume = 3;						
					}
					else
					{
						// End:0x568
						if(__NFUN_130__(__NFUN_178__(0.8000000, fOggVoiceVolume), __NFUN_176__(fOggVoiceVolume, 1.0000000)))
						{
							iTutorialVolume = 4;							
						}
						else
						{
							// End:0x57F
							if(__NFUN_178__(1.0000000, fOggVoiceVolume))
							{
								iTutorialVolume = 5;
							}
						}
					}
				}
			}
		}
		Class'NWindow.UIAPI_SLIDERCTRL'.static.SetCurrentTick("OptionWnd.TutorialVolumeSliderCtrl", iTutorialVolume);
		m_iPrevSoundTick = iSoundVolume;
		m_iPrevMusicTick = iMusicVolume;
		m_iPrevSystemTick = iSystemVolume;
		m_iPrevTutorialTick = iTutorialVolume;		
	}
	else
	{
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.EffectVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.MusicVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.SystemVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.TutorialVolumeSliderCtrl");
	}
	// End:0x7E4
	if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.mutecheckbox"))
	{
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.EffectVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.MusicVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.SystemVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.TutorialVolumeSliderCtrl");
		SetSoundVolume(0.0000000);
		SetMusicVolume(0.0000000);
		SetWavVoiceVolume(0.0000000);
		SetOggVoiceVolume(0.0000000);
		SetOptionBool("Audio", "AudioMuteOn", true);		
	}
	else
	{
		Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.EffectVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.MusicVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.SystemVolumeSliderCtrl");
		Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.TutorialVolumeSliderCtrl");
	}
	return;
}

function InitGameOption()
{
	local bool bShowOtherPCName;
	local int nOption;
	local bool bOption;

	bOption = GetOptionBool("Game", "TransparencyMode");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.OpacityBox", bOption);
	bOption = GetOptionBool("Game", "IsNative");
	// End:0xA0
	if(bOption)
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.LanguageBox", 0);		
	}
	else
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.LanguageBox", 1);
	}
	bOption = GetOptionBool("Game", "MyName");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.NameBox0", bOption);
	bOption = GetOptionBool("Game", "NPCName");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.NameBox1", bOption);
	bShowOtherPCName = GetOptionBool("Game", "GroupName");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.NameBox2", bShowOtherPCName);
	bOption = GetOptionBool("Game", "PledgeMemberName");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.NameBox3", bOption);
	bOption = GetOptionBool("Game", "PartyMemberName");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.NameBox4", bOption);
	bOption = GetOptionBool("Game", "OtherPCName");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.NameBox5", bOption);
	// End:0x2F0
	if(bShowOtherPCName)
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.NameBox3");
		Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.NameBox4");
		Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.NameBox5");		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.NameBox3");
		Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.NameBox4");
		Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.NameBox5");
	}
	bOption = GetOptionBool("Game", "EnterChatting");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.EnterChatBox", bOption);
	bOption = GetOptionBool("Game", "OldChatting");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.OldChatBox", bOption);
	// End:0x4B1
	if(IsUseKeyCrypt())
	{
		// End:0x42F
		if(IsCheckKeyCrypt())
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.KeyboardBox", true);			
		}
		else
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.KeyboardBox", false);
		}
		// End:0x488
		if(IsEnableKeyCrypt())
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.KeyboardBox");			
		}
		else
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.KeyboardBox");
		}		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.KeyboardBox", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.KeyboardBox");
	}
	bOption = GetOptionBool("Game", "UseJoystick");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.JoypadBox", bOption);
	// End:0x578
	if(CanUseJoystick())
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.JoypadBox");		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.JoypadBox");
	}
	bOption = GetOptionBool("Game", "AutoTrackingPawn");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.CameraBox", bOption);
	bOption = GetOptionBool("Video", "UseColorCursor");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.CursorBox", bOption);
	bOption = GetOptionBool("Game", "ArrowMode");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.ArrowBox", bOption);
	bOption = GetOptionBool("Game", "ShowZoneTitle");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.ZoneNameBox", bOption);
	bOption = GetOptionBool("Game", "ShowGameTipMsg");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.GametipBox", bOption);
	bOption = GetOptionBool("Game", "IsRejectingDuel");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.DuelBox", bOption);
	bOption = GetOptionBool("Game", "HideDropItem");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.DropItemBox", bOption);
	nOption = GetOptionInt("Game", "PartyLooting");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.LootingBox", nOption);
	RefreshLootingBox();
	return;
}

function OnClickCheckBox(string strID)
{
	Debug(strID);
	switch(strID)
	{
		// End:0x11D
		case "NameBox2":
			// End:0xB1
			if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.NameBox2"))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.NameBox3");
				Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.NameBox4");
				Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.NameBox5");				
			}
			else
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.NameBox3");
				Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.NameBox4");
				Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.NameBox5");
			}
			// End:0x3FD
			break;
		// End:0x1EA
		case "SystemMsgBox":
			// End:0x1A1
			if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.SystemMsgBox"))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.DamageBox");
				Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("OptionWnd.ItemBox");				
			}
			else
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.DamageBox");
				Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("OptionWnd.ItemBox");
			}
			// End:0x3FD
			break;
		// End:0x22F
		case "FrameBox":
			// End:0x226
			if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.FrameBox"))
			{
				MinFrameRateOn();				
			}
			else
			{
				MinFrameRateOff();
			}
			// End:0x3FD
			break;
		// End:0x3FA
		case "mutecheckbox":
			// End:0x332
			if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.mutecheckbox"))
			{
				Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.EffectVolumeSliderCtrl");
				Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.MusicVolumeSliderCtrl");
				Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.SystemVolumeSliderCtrl");
				Class'NWindow.UIAPI_SLIDERCTRL'.static.DisableWindow("OptionWnd.TutorialVolumeSliderCtrl");				
			}
			else
			{
				Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.EffectVolumeSliderCtrl");
				Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.MusicVolumeSliderCtrl");
				Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.SystemVolumeSliderCtrl");
				Class'NWindow.UIAPI_SLIDERCTRL'.static.EnableWindow("OptionWnd.TutorialVolumeSliderCtrl");
			}
			// End:0x3FD
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnShow()
{
	bShow = true;
	InitVideoOption();
	InitAudioOption();
	InitGameOption();
	return;
}

function OnHide()
{
	bShow = false;
	return;
}

function ApplyVideoOption()
{
	local bool bKeepMinFrameRate, bTrilinear;
	local int nTextureDetail, nModelDetail, nMotionDetail, nTerrainClippingRange, nPawnClippingRange, nReflectionEffect,
		nHDR, nWeatherEffect, nSelectedNum;

	local float fGamma;
	local bool bRenderDeco, bShadow;
	local int nRenderActorLimit, nResolutionIndex, nRefreshRateIndex;
	local bool bIsChecked;

	bTrilinear = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.TriBox");
	SetOptionBool("Video", "UseTrilinear", bTrilinear);
	nTextureDetail = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.TextureBox");
	SetOptionInt("Video", "TextureDetail", nTextureDetail);
	nModelDetail = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.ModelBox");
	SetOptionInt("Video", "ModelDetail", nModelDetail);
	nMotionDetail = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.AnimBox");
	SetOptionInt("Video", "SkipAnim", nMotionDetail);
	nPawnClippingRange = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.CharBox");
	SetOptionInt("Video", "PawnClippingRange", nPawnClippingRange);
	nTerrainClippingRange = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.TerrainBox");
	SetOptionInt("Video", "TerrainClippingRange", nTerrainClippingRange);
	nSelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.GammaBox");
	switch(nSelectedNum)
	{
		// End:0x203
		case 0:
			fGamma = 1.2000000;
			// End:0x251
			break;
		// End:0x215
		case 1:
			fGamma = 1.0000000;
			// End:0x251
			break;
		// End:0x228
		case 2:
			fGamma = 0.8000000;
			// End:0x251
			break;
		// End:0x23B
		case 3:
			fGamma = 0.6000000;
			// End:0x251
			break;
		// End:0x24E
		case 4:
			fGamma = 0.4000000;
			// End:0x251
			break;
		// End:0xFFFF
		default:
			break;
	}
	SetOptionFloat("Video", "Gamma", fGamma);
	nHDR = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.HDRBox");
	SetOptionInt("Video", "PostProc", nHDR);
	bRenderDeco = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.DecoBox");
	SetOptionBool("Video", "RenderDeco", bRenderDeco);
	bShadow = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.ShadowBox");
	SetOptionBool("Video", "PawnShadow", bShadow);
	nReflectionEffect = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.ReflectBox");
	SetOptionInt("L2WaterEffect", "EffectType", nReflectionEffect);
	// End:0x3C1
	if(__NFUN_154__(0, nReflectionEffect))
	{
		SetOptionBool("L2WaterEffect", "IsUseEffect", false);		
	}
	else
	{
		SetOptionBool("L2WaterEffect", "IsUseEffect", true);
	}
	nSelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.PawnBox");
	switch(nSelectedNum)
	{
		// End:0x422
		case 5:
			nRenderActorLimit = 1;
			// End:0x473
			break;
		// End:0x432
		case 4:
			nRenderActorLimit = 2;
			// End:0x473
			break;
		// End:0x442
		case 3:
			nRenderActorLimit = 3;
			// End:0x473
			break;
		// End:0x452
		case 2:
			nRenderActorLimit = 4;
			// End:0x473
			break;
		// End:0x461
		case 1:
			nRenderActorLimit = 5;
			// End:0x473
			break;
		// End:0x470
		case 0:
			nRenderActorLimit = 6;
			// End:0x473
			break;
		// End:0xFFFF
		default:
			break;
	}
	SetOptionInt("Video", "RenderActorLimited", nRenderActorLimit);
	nSelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.AABox");
	SetOptionInt("Video", "AntiAliasing", nSelectedNum);
	nSelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.CaptureBox");
	SetOptionInt("Game", "ScreenShotQuality", nSelectedNum);
	nResolutionIndex = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.ResBox");
	nRefreshRateIndex = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.RefreshRateBox");
	SetResolution(nResolutionIndex, nRefreshRateIndex);
	nSelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.WeatherEffectComboBox");
	switch(nSelectedNum)
	{
		// End:0x5DF
		case 0:
			nWeatherEffect = 0;
			// End:0x610
			break;
		// End:0x5ED
		case 1:
			nWeatherEffect = 1;
			// End:0x610
			break;
		// End:0x5FD
		case 2:
			nWeatherEffect = 2;
			// End:0x610
			break;
		// End:0x60D
		case 3:
			nWeatherEffect = 3;
			// End:0x610
			break;
		// End:0xFFFF
		default:
			break;
	}
	SetOptionInt("Video", "WeatherEffect", nWeatherEffect);
	bIsChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.GPUAnimationCheckBox");
	SetOptionBool("Video", "GPUAnimation", bIsChecked);
	bKeepMinFrameRate = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.FrameBox");
	SetOptionBool("Video", "IsKeepMinFrameRate", bKeepMinFrameRate);
	// End:0x746
	if(bKeepMinFrameRate)
	{
		Debug("KeepMinFrameRate");
		SetTextureDetail(2);
		SetModelingDetail(1);
		SetMotionDetail(1);
		SetShadow(false);
		SetBackgroundEffect(false);
		SetTerrainClippingRange(4);
		SetPawnClippingRange(4);
		SetReflectionEffect(0);
		SetHDR(0);
		SetWeatherEffect(0);		
	}
	else
	{
		Debug(__NFUN_112__("Not KeepMinFrameRate nTextureDetail=", string(nTextureDetail)));
		SetTextureDetail(nTextureDetail);
		SetModelingDetail(nModelDetail);
		SetMotionDetail(nMotionDetail);
		SetShadow(bShadow);
		SetBackgroundEffect(bRenderDeco);
		SetTerrainClippingRange(nTerrainClippingRange);
		SetPawnClippingRange(nPawnClippingRange);
		SetReflectionEffect(nReflectionEffect);
		SetHDR(nHDR);
		SetWeatherEffect(nWeatherEffect);
	}
	InitVideoOption();
	return;
}

function ApplyAudioOption()
{
	local float fSoundVolume, fMusicVolume, fWavVoiceVolume, fOggVoiceVolume;
	local int iSoundTick, iMusicTick, iSystemTick, iTutorialTick;

	// End:0x0D
	if(__NFUN_129__(CanUseAudio()))
	{
		return;
	}
	iSoundTick = Class'NWindow.UIAPI_SLIDERCTRL'.static.GetCurrentTick("OptionWnd.EffectVolumeSliderCtrl");
	fSoundVolume = GetVolumeFromSliderTick(iSoundTick);
	SetOptionFloat("Audio", "SoundVolume", fSoundVolume);
	gSoundVolume = fSoundVolume;
	iMusicTick = Class'NWindow.UIAPI_SLIDERCTRL'.static.GetCurrentTick("OptionWnd.MusicVolumeSliderCtrl");
	fMusicVolume = GetVolumeFromSliderTick(iMusicTick);
	SetOptionFloat("Audio", "MusicVolume", fMusicVolume);
	gMusicVolume = fMusicVolume;
	iSystemTick = Class'NWindow.UIAPI_SLIDERCTRL'.static.GetCurrentTick("OptionWnd.SystemVolumeSliderCtrl");
	fWavVoiceVolume = GetVolumeFromSliderTick(iSystemTick);
	SetOptionFloat("Audio", "WavVoiceVolume", fWavVoiceVolume);
	gWavVoiceVolume = fWavVoiceVolume;
	iTutorialTick = Class'NWindow.UIAPI_SLIDERCTRL'.static.GetCurrentTick("OptionWnd.TutorialVolumeSliderCtrl");
	fOggVoiceVolume = GetVolumeFromSliderTick(iTutorialTick);
	SetOptionFloat("Audio", "OggVoiceVolume", fOggVoiceVolume);
	gOggVoiceVolume = fOggVoiceVolume;
	m_iPrevSoundTick = iSoundTick;
	m_iPrevMusicTick = iMusicTick;
	m_iPrevSystemTick = iSystemTick;
	m_iPrevTutorialTick = iTutorialTick;
	// End:0x27C
	if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.mutecheckbox"))
	{
		SetSoundVolume(0.0000000);
		SetMusicVolume(0.0000000);
		SetWavVoiceVolume(0.0000000);
		SetOggVoiceVolume(0.0000000);
		SetOptionBool("Audio", "AudioMuteOn", true);		
	}
	else
	{
		SetOptionBool("Audio", "AudioMuteOn", false);
	}
	return;
}

function ApplyGameOption()
{
	local int nSelectedNum;
	local bool bChecked;

	nSelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.LanguageBox");
	// End:0x51
	if(__NFUN_154__(0, nSelectedNum))
	{
		SetOptionBool("Game", "IsNative", true);		
	}
	else
	{
		// End:0x73
		if(__NFUN_154__(1, nSelectedNum))
		{
			SetOptionBool("Game", "IsNative", false);
		}
	}
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.NameBox0");
	SetOptionBool("Game", "MyName", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.NameBox1");
	SetOptionBool("Game", "NPCName", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.NameBox3");
	SetOptionBool("Game", "PledgeMemberName", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.NameBox4");
	SetOptionBool("Game", "PartyMemberName", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.NameBox5");
	SetOptionBool("Game", "OtherPCName", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.NameBox2");
	SetOptionBool("Game", "GroupName", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.OpacityBox");
	SetOptionBool("Game", "TransparencyMode", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.ArrowBox");
	SetOptionBool("Game", "ArrowMode", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.CameraBox");
	SetOptionBool("Game", "AutoTrackingPawn", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.EnterChatBox");
	SetOptionBool("Game", "EnterChatting", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.OldChatBox");
	SetOptionBool("Game", "OldChatting", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.ZoneNameBox");
	SetOptionBool("Game", "ShowZoneTitle", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.GametipBox");
	SetOptionBool("Game", "ShowGameTipMsg", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.DuelBox");
	SetOptionBool("Game", "IsRejectingDuel", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.DropItemBox");
	SetOptionBool("Game", "HideDropItem", bChecked);
	// End:0x54E
	if(Class'NWindow.UIAPI_WINDOW'.static.IsEnableWindow("OptionWnd.LootingBox"))
	{
		nSelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("OptionWnd.LootingBox");
		SetOptionInt("Game", "PartyLooting", nSelectedNum);
	}
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.CursorBox");
	SetOptionBool("Video", "UseColorCursor", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.SystemMsgBox");
	SetOptionBool("Game", "SystemMsgWnd", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.DamageBox");
	SetOptionBool("Game", "SystemMsgWndDamage", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.ItemBox");
	SetOptionBool("Game", "SystemMsgWndExpendableItem", bChecked);
	// End:0x6E5
	if(CanUseJoystick())
	{
		bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.JoypadBox");
		SetOptionBool("Game", "UseJoystick", bChecked);
	}
	// End:0x727
	if(IsUseKeyCrypt())
	{
		bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("OptionWnd.KeyboardBox");
		SetKeyCrypt(bChecked);
	}
	return;
}

function OnClickButton(string strID)
{
	Debug(strID);
	switch(strID)
	{
		// End:0x25
		case "VideoCancelBtn":
		// End:0x38
		case "AudioCancelBtn":
		// End:0x110
		case "GameCancelBtn":
			SetOptionInt("FirstRun", "FirstRun", 2);
			OnModifyCurrentTickSliderCtrl("EffectVolumeSliderCtrl", m_iPrevSoundTick);
			OnModifyCurrentTickSliderCtrl("MusicVolumeSliderCtrl", m_iPrevMusicTick);
			OnModifyCurrentTickSliderCtrl("SystemVolumeSliderCtrl", m_iPrevSystemTick);
			OnModifyCurrentTickSliderCtrl("TutorialVolumeSliderCtrl", m_iPrevTutorialTick);
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OptionWnd");
			// End:0x1EF
			break;
		// End:0x11F
		case "VideoOKBtn":
		// End:0x12E
		case "AudioOKBtn":
		// End:0x187
		case "GameOKBtn":
			ApplyVideoOption();
			ApplyAudioOption();
			ApplyGameOption();
			SetOptionInt("FirstRun", "FirstRun", 2);
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OptionWnd");
			// End:0x1EF
			break;
		// End:0x199
		case "VideoApplyBtn":
		// End:0x1AB
		case "AudioApplyBtn":
		// End:0x1D1
		case "GameApplyBtn":
			ApplyVideoOption();
			ApplyAudioOption();
			ApplyGameOption();
			// End:0x1EF
			break;
		// End:0x1EC
		case "WindowInitBtn":
			SetDefaultPosition();
			// End:0x1EF
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function float GetVolumeFromSliderTick(int iTick)
{
	local float fReturnVolume;

	switch(iTick)
	{
		// End:0x19
		case 0:
			fReturnVolume = 0.0000000;
			// End:0x7A
			break;
		// End:0x2B
		case 1:
			fReturnVolume = 0.2000000;
			// End:0x7A
			break;
		// End:0x3E
		case 2:
			fReturnVolume = 0.4000000;
			// End:0x7A
			break;
		// End:0x51
		case 3:
			fReturnVolume = 0.6000000;
			// End:0x7A
			break;
		// End:0x64
		case 4:
			fReturnVolume = 0.8000000;
			// End:0x7A
			break;
		// End:0x77
		case 5:
			fReturnVolume = 1.0000000;
			// End:0x7A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return fReturnVolume;
	return;
}

function MinFrameRateOn()
{
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.TextureBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.ModelBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.AnimBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.ShadowBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.DecoBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.TerrainBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.CharBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.ReflectBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.HDRBox");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.WeatherEffectComboBox");
	return;
}

function MinFrameRateOff()
{
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.TextureBox");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.ModelBox");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.AnimBox");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.ShadowBox");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.DecoBox");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.TerrainBox");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.CharBox");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.WeatherEffectComboBox");
	// End:0x181
	if(__NFUN_150__(nPixelShaderVersion, 12))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.ReflectBox");
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.ReflectBox", 0);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.ReflectBox");
	}
	// End:0x1E4
	if(__NFUN_130__(__NFUN_153__(nPixelShaderVersion, 20), __NFUN_153__(nVertexShaderVersion, 20)))
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("OptionWnd.HDRBox");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("OptionWnd.HDRBox");
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("OptionWnd.HDRBox", 0);
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	local bool bMinFrameRate;

	switch(a_EventID)
	{
		// End:0x81
		case 510:
			bMinFrameRate = GetOptionBool("Video", "IsKeepMinFrameRate");
			Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("OptionWnd.FrameBox", bMinFrameRate);
			// End:0x72
			if(bMinFrameRate)
			{
				MinFrameRateOn();				
			}
			else
			{
				MinFrameRateOff();
			}
			ApplyVideoOption();
			// End:0xC7
			break;
		// End:0x92
		case 520:
			RefreshLootingBox();
			// End:0xC7
			break;
		// End:0xAB
		case 1550:
			m_bPartyMatchRoomState = true;
			RefreshLootingBox();
			// End:0xC7
			break;
		// End:0xC4
		case 1560:
			m_bPartyMatchRoomState = false;
			RefreshLootingBox();
			// End:0xC7
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnComboBoxItemSelected(string sName, int Index)
{
	Debug(sName);
	switch(sName)
	{
		// End:0x46
		case "ResBox":
			ResetRefreshRate(ResolutionList[Index].nWidth, ResolutionList[Index].nHeight);
			// End:0x49
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnModifyCurrentTickSliderCtrl(string strID, int iCurrentTick)
{
	local float fVolume;

	fVolume = GetVolumeFromSliderTick(iCurrentTick);
	switch(strID)
	{
		// End:0x55
		case "EffectVolumeSliderCtrl":
			SetOptionFloat("Audio", "SoundVolume", fVolume);
			// End:0x130
			break;
		// End:0xAB
		case "MusicVolumeSliderCtrl":
			// End:0x89
			if(__NFUN_180__(fVolume, 0.0000000))
			{
				fVolume = 0.0050000;
			}
			SetOptionFloat("Audio", "MusicVolume", fVolume);
			// End:0x130
			break;
		// End:0xEB
		case "SystemVolumeSliderCtrl":
			SetOptionFloat("Audio", "WavVoiceVolume", fVolume);
			// End:0x130
			break;
		// End:0x12D
		case "TutorialVolumeSliderCtrl":
			SetOptionFloat("Audio", "OggVoiceVolume", fVolume);
			// End:0x130
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
