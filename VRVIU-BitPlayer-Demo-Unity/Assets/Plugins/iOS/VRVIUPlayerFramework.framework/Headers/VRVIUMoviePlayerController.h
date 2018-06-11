/*
 * VRVIUMoviePlayerController.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 */

#import "VRVIUMediaPlayback.h"
#import "VRVIUMonitor.h"
#import "VRVIUOptions.h"
#import "VRVIUSDLGLViewProtocol.h"

// media meta
#define k_VRVIUM_KEY_FORMAT         @"format"
#define k_VRVIUM_KEY_DURATION_US    @"duration_us"
#define k_VRVIUM_KEY_START_US       @"start_us"
#define k_VRVIUM_KEY_BITRATE        @"bitrate"

// stream meta
#define k_VRVIUM_KEY_TYPE           @"type"
#define k_VRVIUM_VAL_TYPE__VIDEO    @"video"
#define k_VRVIUM_VAL_TYPE__AUDIO    @"audio"
#define k_VRVIUM_VAL_TYPE__UNKNOWN  @"unknown"

#define k_VRVIUM_KEY_CODEC_NAME      @"codec_name"
#define k_VRVIUM_KEY_CODEC_PROFILE   @"codec_profile"
#define k_VRVIUM_KEY_CODEC_LONG_NAME @"codec_long_name"

// stream: video
#define k_VRVIUM_KEY_WIDTH          @"width"
#define k_VRVIUM_KEY_HEIGHT         @"height"
#define k_VRVIUM_KEY_FPS_NUM        @"fps_num"
#define k_VRVIUM_KEY_FPS_DEN        @"fps_den"
#define k_VRVIUM_KEY_TBR_NUM        @"tbr_num"
#define k_VRVIUM_KEY_TBR_DEN        @"tbr_den"
#define k_VRVIUM_KEY_SAR_NUM        @"sar_num"
#define k_VRVIUM_KEY_SAR_DEN        @"sar_den"
// stream: audio
#define k_VRVIUM_KEY_SAMPLE_RATE    @"sample_rate"
#define k_VRVIUM_KEY_CHANNEL_LAYOUT @"channel_layout"

#define kk_VRVIUM_KEY_STREAMS       @"streams"

typedef enum VRVIULogLevel {
    k_VRVIU_LOG_UNKNOWN = 0,
    k_VRVIU_LOG_DEFAULT = 1,

    k_VRVIU_LOG_VERBOSE = 2,
    k_VRVIU_LOG_DEBUG   = 3,
    k_VRVIU_LOG_INFO    = 4,
    k_VRVIU_LOG_WARN    = 5,
    k_VRVIU_LOG_ERROR   = 6,
    k_VRVIU_LOG_FATAL   = 7,
    k_VRVIU_LOG_SILENT  = 8,
} VRVIULogLevel;

@interface VRVIUMoviePlayerController : NSObject <VRVIUMediaPlayback>

- (id)initWithContentURL:(NSURL *)aUrl
             withOptions:(VRVIUOptions *)options;

- (id)initWithContentURLString:(NSString *)aUrlString
                   withOptions:(VRVIUOptions *)options;

- (id)initWithMoreContent:(NSURL *)aUrl
             withOptions:(VRVIUOptions *)options
              withGLView:(UIView<VRVIUSDLGLViewProtocol> *)glView;

- (id)initWithMoreContentString:(NSString *)aUrlString
                 withOptions:(VRVIUOptions *)options
                  withGLView:(UIView<VRVIUSDLGLViewProtocol> *)glView;
// add by felix 2018.4.24
#if defined(__APPLE__)
- (CVPixelBufferRef)getCurFrame;
#endif
// add end
- (void)prepareToPlay;
- (void)play;
- (void)pause;
- (void)stop;
- (BOOL)isPlaying;
- (int64_t)trafficStatistic;
- (float)dropFrameRate;

- (void)setPauseInBackground:(BOOL)pause;
- (BOOL)isVideoToolboxOpen;

- (void)setHudValue:(NSString *)value forKey:(NSString *)key;



+ (void)setLogReport:(BOOL)preferLogReport;
+ (void)setLogLevel:(VRVIULogLevel)logLevel;
+ (BOOL)checkIfFFmpegVersionMatch:(BOOL)showAlert;
+ (BOOL)checkIfPlayerVersionMatch:(BOOL)showAlert
                            version:(NSString *)version;

@property(nonatomic, readonly) CGFloat fpsInMeta;
@property(nonatomic, readonly) CGFloat fpsAtOutput;
@property(nonatomic) BOOL shouldShowHudView;

- (void)setOptionValue:(NSString *)value
                forKey:(NSString *)key
            ofCategory:(VRVIUOptionCategory)category;

- (void)setOptionIntValue:(int64_t)value
                   forKey:(NSString *)key
               ofCategory:(VRVIUOptionCategory)category;



- (void)setFormatOptionValue:       (NSString *)value forKey:(NSString *)key;
- (void)setCodecOptionValue:        (NSString *)value forKey:(NSString *)key;
- (void)setSwsOptionValue:          (NSString *)value forKey:(NSString *)key;
- (void)setPlayerOptionValue:       (NSString *)value forKey:(NSString *)key;

- (void)setFormatOptionIntValue:    (int64_t)value forKey:(NSString *)key;
- (void)setCodecOptionIntValue:     (int64_t)value forKey:(NSString *)key;
- (void)setSwsOptionIntValue:       (int64_t)value forKey:(NSString *)key;
- (void)setPlayerOptionIntValue:    (int64_t)value forKey:(NSString *)key;

@property (nonatomic, retain) id<VRVIUMediaUrlOpenDelegate> segmentOpenDelegate;
@property (nonatomic, retain) id<VRVIUMediaUrlOpenDelegate> tcpOpenDelegate;
@property (nonatomic, retain) id<VRVIUMediaUrlOpenDelegate> httpOpenDelegate;
@property (nonatomic, retain) id<VRVIUMediaUrlOpenDelegate> liveOpenDelegate;

@property (nonatomic, retain) id<VRVIUMediaNativeInvokeDelegate> nativeInvokeDelegate;

- (void)didShutdown;

#pragma mark KVO properties
@property (nonatomic, readonly) VRVIUMonitor *monitor;

@end

#define VRVIU_FF_IO_TYPE_READ (1)
void VRVIUIOStatDebugCallback(const char *url, int type, int bytes);
void VRVIUIOStatRegister(void (*cb)(const char *url, int type, int bytes));

void VRVIUIOStatCompleteDebugCallback(const char *url,
                                      int64_t read_bytes, int64_t total_size,
                                      int64_t elpased_time, int64_t total_duration);
void VRVIUIOStatCompleteRegister(void (*cb)(const char *url,
                                            int64_t read_bytes, int64_t total_size,
                                            int64_t elpased_time, int64_t total_duration));
