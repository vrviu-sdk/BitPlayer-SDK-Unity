/*
 * VRVIUMediaPlayback.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 */

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

typedef NS_ENUM(NSInteger, VRVIUMPMovieScalingMode) {
    VRVIUMPMovieScalingModeNone,       // No scaling
    VRVIUMPMovieScalingModeAspectFit,  // Uniform scale until one dimension fits
    VRVIUMPMovieScalingModeAspectFill, // Uniform scale until the movie fills the visible bounds. One dimension may have clipped contents
    VRVIUMPMovieScalingModeFill        // Non-uniform scale. Both render dimensions will exactly match the visible bounds
};

typedef NS_ENUM(NSInteger, VRVIUMPMoviePlaybackState) {
    VRVIUMPMoviePlaybackStateStopped,
    VRVIUMPMoviePlaybackStatePlaying,
    VRVIUMPMoviePlaybackStatePaused,
    VRVIUMPMoviePlaybackStateInterrupted,
    VRVIUMPMoviePlaybackStateSeekingForward,
    VRVIUMPMoviePlaybackStateSeekingBackward
};

typedef NS_OPTIONS(NSUInteger, VRVIUMPMovieLoadState) {
    VRVIUMPMovieLoadStateUnknown        = 0,
    VRVIUMPMovieLoadStatePlayable       = 1 << 0,
    VRVIUMPMovieLoadStatePlaythroughOK  = 1 << 1, // Playback will be automatically started in this state when shouldAutoplay is YES
    VRVIUMPMovieLoadStateStalled        = 1 << 2, // Playback will be automatically paused in this state, if started
};

typedef NS_ENUM(NSInteger, VRVIUMPMovieFinishReason) {
    VRVIUMPMovieFinishReasonPlaybackEnded,
    VRVIUMPMovieFinishReasonPlaybackError,
    VRVIUMPMovieFinishReasonUserExited
};

// -----------------------------------------------------------------------------
// Thumbnails

typedef NS_ENUM(NSInteger, VRVIUMPMovieTimeOption) {
    VRVIUMPMovieTimeOptionNearestKeyFrame,
    VRVIUMPMovieTimeOptionExact
};

@protocol VRVIUMediaPlayback;

#pragma mark VRVIUMediaPlayback

@protocol VRVIUMediaPlayback <NSObject>
// add by felix 2018.4.24
#if defined(__APPLE__)
- (CVPixelBufferRef)getCurFrame;
- (void)unity_lock;
- (void)unity_unlock;
- (void)unity_ready_release;
#endif
// add end
- (void)prepareToPlay;
- (void)play;
- (void)pause;
- (void)stop;
- (BOOL)isPlaying;
- (void)shutdown;
- (void)setPauseInBackground:(BOOL)pause;



@property(nonatomic, readonly)  UIView *view;
@property(nonatomic)            NSTimeInterval currentPlaybackTime;
@property(nonatomic, readonly)  NSTimeInterval duration;
@property(nonatomic, readonly)  NSTimeInterval playableDuration;
@property(nonatomic, readonly)  NSInteger bufferingProgress;

@property(nonatomic, readonly)  BOOL isPreparedToPlay;
@property(nonatomic, readonly)  VRVIUMPMoviePlaybackState playbackState;
@property(nonatomic, readonly)  VRVIUMPMovieLoadState loadState;
@property(nonatomic, readonly) int isSeekBuffering;
@property(nonatomic, readonly) int isAudioSync;
@property(nonatomic, readonly) int isVideoSync;

@property(nonatomic, readonly) int64_t numberOfBytesTransferred;

@property(nonatomic, readonly) CGSize naturalSize;
@property(nonatomic) VRVIUMPMovieScalingMode scalingMode;
@property(nonatomic) BOOL shouldAutoplay;

@property (nonatomic) BOOL allowsMediaAirPlay;
@property (nonatomic) BOOL isDanmakuMediaAirPlay;
@property (nonatomic, readonly) BOOL airPlayMediaActive;

@property (nonatomic) float playbackRate;
@property (nonatomic) float playbackVolume;


- (UIImage *)thumbnailImageAtCurrentTime;

#pragma mark Notifications

#ifdef __cplusplus
#define VRVIU_EXTERN extern "C" __attribute__((visibility ("default")))
#else
#define VRVIU_EXTERN extern __attribute__((visibility ("default")))
#endif

// -----------------------------------------------------------------------------
//  MPMediaPlayback.h  VRVIU

// Posted when the prepared state changes of an object conforming to the MPMediaPlayback protocol changes.
// This supersedes MPMoviePlayerContentPreloadDidFinishNotification.
VRVIU_EXTERN NSString *const VRVIUMPMediaPlaybackIsPreparedToPlayDidChangeNotification;

// -----------------------------------------------------------------------------
//  MPMoviePlayerController.h
//  Movie Player Notifications

// Posted when the scaling mode changes.
VRVIU_EXTERN NSString* const VRVIUMPMoviePlayerScalingModeDidChangeNotification;

// Posted when movie playback ends or a user exits playback.
VRVIU_EXTERN NSString* const VRVIUMPMoviePlayerPlaybackDidFinishNotification;
VRVIU_EXTERN NSString* const VRVIUMPMoviePlayerPlaybackDidFinishReasonUserInfoKey; // NSNumber (VRVIUMPMovieFinishReason)

// Posted when the playback state changes, either programatically or by the user.
VRVIU_EXTERN NSString* const VRVIUMPMoviePlayerPlaybackStateDidChangeNotification;

// Posted when the network load state changes.
VRVIU_EXTERN NSString* const VRVIUMPMoviePlayerLoadStateDidChangeNotification;

// Posted when the movie player begins or ends playing video via AirPlay.
VRVIU_EXTERN NSString* const VRVIUMPMoviePlayerIsAirPlayVideoActiveDidChangeNotification;

// -----------------------------------------------------------------------------
// Movie Property Notifications

// Calling -prepareToPlay on the movie player will begin determining movie properties asynchronously.
// These notifications are posted when the associated movie property becomes available.
VRVIU_EXTERN NSString* const VRVIUMPMovieNaturalSizeAvailableNotification;

// -----------------------------------------------------------------------------
//  Extend Notifications

VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerVideoDecoderOpenNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerFirstVideoFrameRenderedNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerFirstAudioFrameRenderedNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerFirstAudioFrameDecodedNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerFirstVideoFrameDecodedNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerOpenInputNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerFindStreamInfoNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerComponentOpenNotification;

VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerDidSeekCompleteNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerDidSeekCompleteTargetKey;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerDidSeekCompleteErrorKey;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerDidAccurateSeekCompleteCurPos;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerAccurateSeekCompleteNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerSeekAudioStartNotification;
VRVIU_EXTERN NSString *const VRVIUMPMoviePlayerSeekVideoStartNotification;

@end

#pragma mark VRVIUMediaUrlOpenDelegate

// Must equal to the defination in VRVIUavformat/VRVIUavformat.h
typedef NS_ENUM(NSInteger, VRVIUMediaEvent) {

    // Notify Events
    VRVIUMediaEvent_WillHttpOpen         = 1,       // attr: url
    VRVIUMediaEvent_DidHttpOpen          = 2,       // attr: url, error, http_code
    VRVIUMediaEvent_WillHttpSeek         = 3,       // attr: url, offset
    VRVIUMediaEvent_DidHttpSeek          = 4,       // attr: url, offset, error, http_code
    // Control Message
    VRVIUMediaCtrl_WillTcpOpen           = 0x20001, // VRVIUMediaUrlOpenData: no args
    VRVIUMediaCtrl_DidTcpOpen            = 0x20002, // VRVIUMediaUrlOpenData: error, family, ip, port, fd
    VRVIUMediaCtrl_WillHttpOpen          = 0x20003, // VRVIUMediaUrlOpenData: url, segmentIndex, retryCounter
    VRVIUMediaCtrl_WillLiveOpen          = 0x20005, // VRVIUMediaUrlOpenData: url, retryCounter
    VRVIUMediaCtrl_WillConcatSegmentOpen = 0x20007, // VRVIUMediaUrlOpenData: url, segmentIndex, retryCounter
};

#define VRVIUMediaEventAttrKey_url            @"url"
#define VRVIUMediaEventAttrKey_host           @"host"
#define VRVIUMediaEventAttrKey_error          @"error"
#define VRVIUMediaEventAttrKey_time_of_event  @"time_of_event"
#define VRVIUMediaEventAttrKey_http_code      @"http_code"
#define VRVIUMediaEventAttrKey_offset         @"offset"
#define VRVIUMediaEventAttrKey_file_size      @"file_size"

// event of VRVIUMediaUrlOpenEvent_xxx
@interface VRVIUMediaUrlOpenData: NSObject

- (id)initWithUrl:(NSString *)url
            event:(VRVIUMediaEvent)event
     segmentIndex:(int)segmentIndex
     retryCounter:(int)retryCounter;

@property(nonatomic, readonly) VRVIUMediaEvent event;
@property(nonatomic, readonly) int segmentIndex;
@property(nonatomic, readonly) int retryCounter;

@property(nonatomic, retain) NSString *url;
@property(nonatomic, assign) int fd;
@property(nonatomic, strong) NSString *msg;
@property(nonatomic) int error; // set a negative value to indicate an error has occured.
@property(nonatomic, getter=isHandled)    BOOL handled;     // auto set to YES if url changed
@property(nonatomic, getter=isUrlChanged) BOOL urlChanged;  // auto set to YES by url changed

@end

@protocol VRVIUMediaUrlOpenDelegate <NSObject>

- (void)willOpenUrl:(VRVIUMediaUrlOpenData*) urlOpenData;

@end

@protocol VRVIUMediaNativeInvokeDelegate <NSObject>

- (int)invoke:(VRVIUMediaEvent)event attributes:(NSDictionary *)attributes;

@end
