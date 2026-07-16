#!/usr/bin/env python3
"""
Generate SOR (Standard OTDR Record) files with random data.
SOR is a binary format defined by Telcordia SR-4731.
"""

import os
import struct
import random

# House numbers for file naming
HOUSE_NUMBERS = [
    "1a1", "1a2", "1a3", "1a4", "1a5", "1a6", "1a7", "1a8", "1a9", "1a10",
    "1a11", "1a12", "1b1", "1b2", "1b3", "1b4", "1b5", "1b6", "1e", "1f",
    "1g", "1h", "1j", "1k", "1l", "1p", "1r", "1q", "1s"
]

OUTPUT_DIR = "/workspace/sor_files"

# File size range in bytes (1.1 MB to 1.7 MB)
MIN_SIZE = int(1.1 * 1024 * 1024)
MAX_SIZE = int(1.7 * 1024 * 1024)

def create_sor_file(filename, target_size):
    """
    Create a SOR file with proper header structure and random data.
    SOR format has a specific header structure followed by data blocks.
    """
    with open(filename, 'wb') as f:
        # SOR file starts with "Map" block identifier
        # Write SOR header - Version 2.0 format
        
        # Map block - contains file structure information
        f.write(b'Map\x00')  # Block ID
        f.write(struct.pack('<H', 2))  # Version number (2)
        
        # Write the number of blocks (we'll have several standard blocks)
        num_blocks = 8
        f.write(struct.pack('<H', num_blocks))
        
        # Block identifiers commonly found in SOR files:
        blocks = [
            b'GenParams',   # General parameters
            b'SupParams',   # Supplier parameters
            b'FxdParams',   # Fixed parameters
            b'KeyEvents',   # Key events (splices, connectors, etc.)
            b'LnkParams',   # Link parameters
            b'DataPts',     # Data points (the actual OTDR trace)
            b'Checksum',    # File checksum
            b'Cksum'        # Alternative checksum block
        ]
        
        # Write block directory entries
        current_offset = 100  # Start data after header
        for block in blocks:
            # Block name (padded to 12 bytes)
            padded_name = block.ljust(12, b'\x00')
            f.write(padded_name)
        
        # GenParams block - General parameters
        f.write(b'\x00' * 20)  # Padding to offset 100
        
        # Write some realistic-looking OTDR parameters
        # Language code
        f.write(b'EN\x00\x00')
        
        # Cable ID (null-terminated string)
        cable_id = b'FIBER_CABLE_001\x00'
        f.write(cable_id)
        
        # Fiber ID
        fiber_id = b'FIBER_01\x00'
        f.write(fiber_id)
        
        # Wavelength in nm (typically 1310, 1550, or 1625)
        wavelengths = [1310, 1550, 1625]
        wavelength = random.choice(wavelengths)
        f.write(struct.pack('<H', wavelength))
        
        # Location A and B
        f.write(b'LOCATION_A\x00'.ljust(24, b'\x00'))
        f.write(b'LOCATION_B\x00'.ljust(24, b'\x00'))
        
        # Cable code
        f.write(b'G.652D\x00'.ljust(16, b'\x00'))
        
        # Build condition (BC, CC, RC)
        f.write(b'BC\x00\x00')
        
        # User offset and operator
        f.write(struct.pack('<i', 0))  # User offset
        f.write(b'OPERATOR\x00'.ljust(24, b'\x00'))
        
        # Comments
        f.write(b'Generated SOR file for testing\x00'.ljust(64, b'\x00'))
        
        # FxdParams block - Fixed parameters
        # Date and time (seconds since 1970)
        import time
        timestamp = int(time.time()) - random.randint(0, 86400 * 365)
        f.write(struct.pack('<I', timestamp))
        
        # Units (km, kft, m, ft, mi)
        f.write(b'km\x00\x00')
        
        # Actual wavelength (in 0.1 nm units)
        f.write(struct.pack('<H', wavelength * 10))
        
        # Acquisition offset and range
        f.write(struct.pack('<i', 0))  # Acquisition offset
        f.write(struct.pack('<i', random.randint(1000, 50000)))  # Range in meters
        
        # Pulse width (ns)
        pulse_widths = [5, 10, 20, 30, 50, 100, 275, 500, 1000, 2500, 5000, 10000, 20000]
        f.write(struct.pack('<H', random.choice(pulse_widths)))
        
        # Resolution (sampling spacing in 0.01m units)
        f.write(struct.pack('<I', random.randint(10, 100)))
        
        # Number of data points
        num_points = random.randint(10000, 50000)
        f.write(struct.pack('<I', num_points))
        
        # Index of refraction (in 0.00001 units, typically ~1.46800)
        f.write(struct.pack('<I', 146800 + random.randint(-500, 500)))
        
        # Backscatter coefficient (in 0.1 dB units)
        f.write(struct.pack('<h', -800 + random.randint(-50, 50)))
        
        # Number of averages
        f.write(struct.pack('<I', random.randint(1000, 65000)))
        
        # Averaging time in seconds
        f.write(struct.pack('<H', random.randint(10, 180)))
        
        # DataPts block header
        # Scale factor and data
        f.write(struct.pack('<H', 1000))  # Scale factor
        
        # Write some simulated OTDR trace data points
        # A typical OTDR trace starts high and decays with distance
        # with occasional spikes for events (splices, connectors, etc.)
        
        current_pos = f.tell()
        bytes_written = current_pos
        
        # Calculate how many bytes we need to reach target size
        remaining_bytes = target_size - bytes_written
        
        # Generate random data that looks somewhat like OTDR measurements
        # OTDR data is typically 16-bit values representing dB levels
        
        # Generate data in chunks for efficiency
        chunk_size = 8192
        while remaining_bytes > 0:
            write_size = min(chunk_size, remaining_bytes)
            # Generate random bytes
            random_data = bytes(random.getrandbits(8) for _ in range(write_size))
            f.write(random_data)
            remaining_bytes -= write_size
        
        # Write a simple checksum at the end
        f.write(struct.pack('<I', 0xDEADBEEF))

def main():
    os.makedirs(OUTPUT_DIR, exist_ok=True)
    
    print(f"Generating {len(HOUSE_NUMBERS)} SOR files...")
    
    for house_num in HOUSE_NUMBERS:
        filename = f"2253XS_{house_num}.sor"
        filepath = os.path.join(OUTPUT_DIR, filename)
        
        # Random size between 1.1 MB and 1.7 MB
        target_size = random.randint(MIN_SIZE, MAX_SIZE)
        
        create_sor_file(filepath, target_size)
        
        actual_size = os.path.getsize(filepath)
        print(f"Created: {filename} ({actual_size / (1024*1024):.2f} MB)")
    
    print(f"\nAll files created in: {OUTPUT_DIR}")

if __name__ == "__main__":
    main()
