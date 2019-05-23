using DataSet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeIndexer {
    
    private List<TimeValue> _content;

    private List<TimeIndexer> _children;

    private int _cellSize;

    private long _start;

    private long _length;

    private int _breaksIn;

    public List<TimeValue> content {
        get {return _content; }
    }

    public int cellSize{
        get {return _cellSize; }
    }

    public long start{
        get {return _start; }
    }

    public long length{
        get {return _length; }
    }

    public long breaksIn{
        get {return _breaksIn; }
    }

    public bool breaked {
        get { return _children.Count > 0; }
    }

    public List<TimeIndexer> children {
        get { return _children; }
    }

    public TimeIndexer(long start, long length, int cellSize = 10, int breaksIn = 2){
        _children = new List<TimeIndexer>();
        _content = new List<TimeValue>();
        _cellSize = cellSize;
        _breaksIn = breaksIn;
        _start = start;
        _length = length;
    }

    public bool Add(TimeValue value){

        if(value.timestamp < _start || value.timestamp > _start + _length){
            return false;
        }

        if(_content.Count == 0){
            _content.Add(value);
        } else {
            for(int i = 0; i<=_content.Count; i++){
                if(i == _content.Count){
                    _content.Add(value);
                    break;
                } else if(_content[i].timestamp > value.timestamp){
                    _content.Insert(i,value);
                    break;
                }
            }
        }

        if(_content.Count > _cellSize && !breaked){
            BreakContent();
        }

        foreach(TimeIndexer indexer in _children){
            if(indexer.Add(value)){
                break;
            }
        }

        return true;
    }

    public TimeValue Get(long timestamp){

        if(timestamp < _start || timestamp > _start + _length){
            return null;
        }

        if(breaked){
            foreach(TimeIndexer indexer in _children){
                TimeValue val = indexer.Get(timestamp);
                if(val != null){
                    return val;
                }
            }
        } else {
            foreach(TimeValue value in _content){
                if(value.timestamp == timestamp){
                    return value;
                }
            }
        }

        return null;

    }

    private void BreakContent(){

        if(breaked){
            Debug.LogError("Can't break an Indexer a second time");
            return;
        }

        long childLength = (long) Math.Ceiling((double) _length / _breaksIn);
        for(int i = 0; i<_breaksIn; i++){
            _children.Add(new TimeIndexer(_start+i*childLength,childLength,_cellSize,_breaksIn));
        }

        foreach(TimeValue value in _content){
            foreach(TimeIndexer indexer in _children){
                if(indexer.Add(value)){
                    break;
                }
            }
        }

    }

}