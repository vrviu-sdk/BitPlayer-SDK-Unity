/*
 * VRVIUFFOptions.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 */

#import <Foundation/Foundation.h>

typedef enum VRVIUOptionCategory {
    kVRVIUOptionCategoryFormat = 1,
    kVRVIUOptionCategoryCodec  = 2,
    kVRVIUOptionCategorySws    = 3,
    kVRVIUOptionCategoryPlayer = 4,
    kVRVIUOptionCategorySwr    = 5,
} VRVIUOptionCategory;

// for codec option 'skip_loop_filter' and 'skip_frame'
typedef enum VRVIUAVDiscard {
    /* We leave some space between them for extensions (drop some
     * keyframes for intra-only or drop just some bidir frames). */
    VRVIU_AVDISCARD_NONE    =-16, ///< discard nothing
    VRVIU_AVDISCARD_DEFAULT =  0, ///< discard useless packets like 0 size packets in avi
    VRVIU_AVDISCARD_NONREF  =  8, ///< discard all non reference
    VRVIU_AVDISCARD_BIDIR   = 16, ///< discard all bidirectional frames
    VRVIU_AVDISCARD_NONKEY  = 32, ///< discard all frames except keyframes
    VRVIU_AVDISCARD_ALL     = 48, ///< discard all
} VRVIUAVDiscard;

/*
typedef struct VRVIUAccount {
    NSString* appID;
    NSString* accessKeyID;
    NSString* bizID;
    NSString* accessKey;
    NSString* host;
};
 */

struct ViuMediaPlayer;

@interface VRVIUOptions : NSObject

+(VRVIUOptions *)optionsByDefault;


-(void)applyTo:(struct ViuMediaPlayer *)mediaPlayer;

- (void)setOptionValue:(NSString *)value
                forKey:(NSString *)key
            ofCategory:(VRVIUOptionCategory)category;

- (void)setOptionIntValue:(int64_t)value
                   forKey:(NSString *)key
               ofCategory:(VRVIUOptionCategory)category;


-(void)setFormatOptionValue:       (NSString *)value forKey:(NSString *)key;
-(void)setCodecOptionValue:        (NSString *)value forKey:(NSString *)key;
-(void)setSwsOptionValue:          (NSString *)value forKey:(NSString *)key;
-(void)setPlayerOptionValue:       (NSString *)value forKey:(NSString *)key;

-(void)setFormatOptionIntValue:    (int64_t)value forKey:(NSString *)key;
-(void)setCodecOptionIntValue:     (int64_t)value forKey:(NSString *)key;
-(void)setSwsOptionIntValue:       (int64_t)value forKey:(NSString *)key;
-(void)setPlayerOptionIntValue:    (int64_t)value forKey:(NSString *)key;

@property(nonatomic) BOOL showHudView;

@end
