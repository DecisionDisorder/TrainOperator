/*
 * adlibr - Library for mobile AD mediation.
 * http://adlibr.com
 * Copyright (c) 2012-2013 Mocoplex, Inc.  All rights reserved.
 * Licensed under the BSD open source license.
 */

/*
 * confirmed compatible with ad@m SDK 2.3.7
 */

package test.adlib.project.ads;

import net.daum.adam.publisher.AdInterstitial;
import net.daum.adam.publisher.AdView.OnAdClosedListener;
import net.daum.adam.publisher.AdView.OnAdFailedListener;
import net.daum.adam.publisher.AdView.OnAdLoadedListener;
import net.daum.adam.publisher.impl.AdError;
import android.app.Activity;
import android.content.Context;
import android.os.Build;
import android.os.Handler;
import android.os.Message;
import android.util.AttributeSet;
import android.widget.LinearLayout;

import com.mocoplex.adlib.AdlibManager;
import com.mocoplex.adlib.SubAdlibAdViewCore;

public class SubAdlibAdViewAdam extends SubAdlibAdViewCore  {
	
	protected net.daum.adam.publisher.AdView ad;
	protected boolean bGotAd = false;
	
	// 여기에 ADAM ID 를 입력하세요.
	protected String adamID = "Adam_ID";
	protected static String adamInterstitialID = "Adam_Interstitial_ID";

	public SubAdlibAdViewAdam(Context context) {
		this(context,null);
	}	
	
	public SubAdlibAdViewAdam(Context context, AttributeSet attrs) {
		super(context, attrs);
		
		initAdamView();
	}
	
	public void initAdamView() {
		ad = new net.daum.adam.publisher.AdView(getContext());
		// 킷캣 디바이스에서 렌더링에 생기는 문제로 인한 예외처리.
		// 에러표시가 생기면 disalbe check하시고 무시하셔도 무방합니다.
		if (Build.VERSION.SDK_INT == 19) {
			ad.setLayerType(LAYER_TYPE_SOFTWARE, null);
		}
		
		// 할당 받은 clientId 설정
		ad.setClientId(adamID);
		// 광고 갱신 시간 : 기본 60초
		ad.setRequestInterval(15);
        
		LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
		ad.setLayoutParams(params);
		
		ad.setOnAdLoadedListener(new OnAdLoadedListener() {
			@Override
			public void OnAdLoaded() {
				
				bGotAd = true;
				// 광고를 받아왔으면 이를 알려 화면에 표시합니다.
				gotAd();
			} });
		
		ad.setOnAdFailedListener(new OnAdFailedListener() {
			@Override
			public void OnAdFailed(AdError arg0, String arg1) {
				
				bGotAd = true;
				failed();
			} });

		this.addView(ad);
	}
	
	// 스케줄러에의해 자동으로 호출됩니다.
	// 실제로 광고를 보여주기 위하여 요청합니다.	
	public void query() {
		bGotAd = false;
		
		if(ad == null)
			initAdamView();
		
		queryAd();

		ad.resume();
		
		// 3초 이상 리스너 응답이 없으면 다음 플랫폼으로 넘어갑니다.
		Handler adHandler = new Handler();
		adHandler.postDelayed(new Runnable() {

			@Override
			public void run() {
				if(bGotAd){
					return;
				}else{
					if(ad != null)
						ad.pause();
					
					failed();
				}
			}
				
		}, 3000);
	}

	// 광고뷰가 사라지는 경우 호출됩니다. 
	public void clearAdView() {
		if(ad != null) {
			this.removeView(ad);
			ad.destroy();
			ad = null;
		}

		super.clearAdView();
	}
	
	public void onResume() {		
		if(ad != null){
			ad.resume();
		}
        
        super.onResume();
	}
	
	public void onPause() {
		if(ad != null){
			ad.pause();
		}
        
        super.onPause();
	}
	
	public void onDestroy() {
		if(ad != null){
			ad.destroy();
			ad = null;
		}
        
        super.onDestroy();
	}
	
	public static void loadInterstitial(Context ctx, final Handler h, final String adlibKey) {
		AdInterstitial mAdInterstitial = new AdInterstitial((Activity)ctx);
 	    mAdInterstitial.setClientId(adamInterstitialID);
 	    mAdInterstitial.setOnAdLoadedListener(new OnAdLoadedListener() {

 			@Override
 			public void OnAdLoaded() {
	 			try{
	 				if(h != null){
	 					h.sendMessage(Message.obtain(h, AdlibManager.DID_SUCCEED, "ADAM"));
	 				}
	 			}catch(Exception e){
	 			}
 					
 			}
 	    });
 	    mAdInterstitial.setOnAdFailedListener(new OnAdFailedListener() {

 			@Override
 			public void OnAdFailed(AdError arg0, String arg1) {
 					
	 			try{
	 				if(h != null){
	 	 				h.sendMessage(Message.obtain(h, AdlibManager.DID_ERROR, "ADAM"));
	 				}
	 			}catch(Exception e){
	 			}
 			}
 	    });
 	    mAdInterstitial.setOnAdClosedListener(new OnAdClosedListener() {

 			@Override
 			public void OnAdClosed() {
 					
	 			try{
	 				if(h != null){
	 					h.sendMessage(Message.obtain(h, AdlibManager.INTERSTITIAL_CLOSED, "ADAM"));
	 				}
	 			}catch(Exception e){
	 			}
 				
 			}
 	    });
 	    	
 	    mAdInterstitial.loadAd();		
	}
}